using System;
using UnityEngine;
using QFramework;
using QFramework.Example;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.AmorHero
{
	#region Command
	public class CheckPlayerWithPlatform : AbstractCommand
	{
		private Vector2 centPosition;
		private Vector2 centerOffset;
		private float checkRadius;
		private LayerMask layerMask;

		public CheckPlayerWithPlatform(Vector2 centPosition, Vector2 centerOffset, float checkRadius, LayerMask layerMask)
		{
			this.centPosition = centPosition;
			this.centerOffset = centerOffset;
			this.checkRadius = checkRadius;
			this.layerMask = layerMask;
		}

		protected override void OnExecute()
		{
			var model = this.GetModel<PlayerModel>();
			model.isOnPlatform.Value = Physics2D.OverlapCircle(centPosition + centerOffset, checkRadius, layerMask);
			if (model.isOnPlatform.Value)
			{
				//Debug.Log("玩家站在地面");
			}
			else
			{
				//Debug.Log("玩家不在地面");
			}
		}
	}
	public class PlayerLevelUp : AbstractCommand
	{
		private int upValue;

		public PlayerLevelUp(int upValue)
		{
			this.upValue = upValue;
		}
		protected override void OnExecute()
		{
			this.GetModel<PlayerModel>().playerLevel.Value += upValue;
			Debug.Log("玩家升级！");
			this.SendEvent<PlayerLevelChangeEvent>();
		}
	}
	#endregion
	#region Event
	public struct PlayerLevelChangeEvent{}
	public struct PlayerHealthChangeEvent{}
	public struct PlayerMaxHealthChangeEvent{}
	public struct PlayerMagicPointChangeEvent{}
	public struct PlayerMaxMagicPointChangeEvent{}
	#endregion
	public class PlayerModel : AbstractModel
	{
		public PlayerData playerData;
		public float jumpForceValue;//向上跳跃时施加力的大小
		public BindableProperty<int> playerHealth = new BindableProperty<int>();//玩家当前生命值
		public BindableProperty<int> playerMaxHealth = new BindableProperty<int>();//玩家最大生命值上限
		public BindableProperty<int> playerMagicPoint = new BindableProperty<int>();//玩家当前意能量
		public BindableProperty<int> playerMaxMagicPoint = new BindableProperty<int>();//玩家最大意能量上限
		public BindableProperty<float> playerRunSpeedValue=new BindableProperty<float>();//玩家移动速度大小
		public BindableProperty<bool> isOnPlatform=new BindableProperty<bool>();//玩家是否在平台上
		public BindableProperty<int> playerLevel=new BindableProperty<int>();//玩家的等级
		public BindableProperty<bool> isAttack=new BindableProperty<bool>();//玩家是否在攻击
		protected override void OnInit()
		{
			AsyncOperationHandle<PlayerData> handle = Addressables.LoadAssetAsync<PlayerData>("Assets/Resource/PlayerData.asset");
			handle.Completed += (operationHandle =>
			{
				playerData = operationHandle.Result;
				GetPlayerDataCore(0);
				playerHealth.Value = 20;
				playerMagicPoint.Value = 20;
				isOnPlatform.Value = true;
				Debug.Log("初始化PlayerModel");
				this.SendEvent<PlayerLevelChangeEvent>();
				this.SendEvent<PlayerHealthChangeEvent>();
			});
		}
		void GetPlayerDataCore(int coreIndex)
		{
			if (playerData.PlayerDataCoreList.Count > coreIndex)
			{
				playerMaxHealth.Value = playerData.PlayerDataCoreList[coreIndex].player_max_health;
				playerMaxMagicPoint.Value = playerData.PlayerDataCoreList[coreIndex].player_max_magicpoint;
				jumpForceValue = playerData.PlayerDataCoreList[coreIndex].player_jump_force_value;
				playerRunSpeedValue.Value = playerData.PlayerDataCoreList[coreIndex].player_runspeed_value;
				playerLevel.Value=playerData.PlayerDataCoreList[coreIndex].player_level;
			}
		}
	}

	public partial class Player : ViewController, IController
	{
		public enum PlayerState
		{
			待机, 跑步, 下蹲, 跳跃,攻击
		}
		public FSM<PlayerState> playerFSM = new FSM<PlayerState>();
		public Vector2 currentDir = new Vector2(1, 0);//主角默认朝向
		public Animator playerAnimator;//玩家动画控制器组件
		public PlayerModel playerModel;
		public Rigidbody2D mRigidBody;
		public float checkPlatformRadius;//检测平台的检测半径
		public Vector2 checkPlatformOffset;//检测中心的偏移量
		public LayerMask platformLayer;//平台的层级
		void Start()
		{
			// Code Here
			playerModel = this.GetModel<PlayerModel>();
			mRigidBody = GetComponent<Rigidbody2D>();
			playerAnimator = GetComponent<Animator>();
			#region 状态
			playerFSM.AddState(PlayerState.待机, new PlayerIdleState(playerFSM, this));
			playerFSM.AddState(PlayerState.跑步, new PlayerRunState(playerFSM, this));
			playerFSM.AddState(PlayerState.下蹲, new PlayerCrouchState(playerFSM, this));
			playerFSM.AddState(PlayerState.跳跃,new PlayerJumpState(playerFSM,this));
			playerFSM.AddState(PlayerState.攻击,new PlayerAttack(playerFSM,this));
			playerFSM.StartState(PlayerState.待机);
			#endregion
			#region 事件注册
			#endregion
		}
		void Update()
		{
			playerFSM.Update();
			//发送检测玩家与平台关系的指令
			this.SendCommand(new CheckPlayerWithPlatform((Vector2)gameObject.transform.position,checkPlatformOffset,checkPlatformRadius,platformLayer));
		}
		void FixedUpdate()
		{
			playerFSM.FixedUpdate();
		}
		void OnDestroy()
		{
			playerFSM.Clear();
		}
		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawSphere((Vector2)gameObject.transform.position+checkPlatformOffset,checkPlatformRadius);
		}

		public void Run(float runX, float runY)
		{
			gameObject.transform.localScale = new Vector3(runX, 1, 0);
			mRigidBody.velocity = new Vector2(runX, runY) * playerModel.playerRunSpeedValue.Value;
			playerAnimator.SetFloat("runSpeed", mRigidBody.velocity.sqrMagnitude);
			currentDir = new Vector2(runX, 0);
		}

		public void AttackFinal()
		{
			Debug.Log("攻击结束");
			playerModel.isAttack.Value = false;
		}
		public IArchitecture GetArchitecture()
		{
			return AmorHeroArchitecture.Interface;
		}
	}

	#region 玩家状态类

	public class PlayerIdleState : AbstractState<Player.PlayerState, Player>
	{
		public PlayerIdleState(FSM<Player.PlayerState> fsm, Player target) : base(fsm, target)
		{
		}
		protected override bool OnCondition()
		{
			return base.OnCondition();
		}
		protected override void OnEnter()
		{
			base.OnEnter();
			//Debug.Log("进入待机模式");
			mTarget.playerAnimator.SetBool("isOnPlatform",true);
			mTarget.gameObject.transform.localScale = new Vector3(mTarget.currentDir.x, 1, 0);
			mTarget.playerAnimator.SetFloat("runSpeed", 0f);
			mTarget.mRigidBody.velocity = Vector2.zero;
		}
		protected override void OnUpdate()
		{
			base.OnUpdate();
			//监听玩家控制
			float runX = Input.GetAxisRaw("Horizontal");
			float runY = Input.GetAxisRaw("Vertical");
			mTarget.gameObject.transform.localScale = new Vector3(mTarget.currentDir.x, 1, 1);
			//只有横向移动，说明玩家是跑步
			if (runX != 0 && runY == 0)
			{
				mTarget.playerFSM.ChangeState(Player.PlayerState.跑步);
			}
			if (Input.GetKeyDown(KeyCode.S))
			{
				mTarget.playerFSM.ChangeState(Player.PlayerState.下蹲);
			}
			if (runY > 0)
			{
				//给刚体施加一个向上的瞬时力；预备进入跳跃
				mTarget.mRigidBody.AddForce(new Vector2(0,mTarget.playerModel.jumpForceValue),ForceMode2D.Impulse);
				mTarget.playerFSM.ChangeState(Player.PlayerState.跳跃);
			}

			if (Input.GetKeyDown(KeyCode.J))
			{
				mFSM.ChangeState(Player.PlayerState.攻击);
			}
		}
		protected override void OnFixedUpdate()
		{
			base.OnFixedUpdate();

		}
		protected override void OnExit()
		{
			base.OnExit();
			//Debug.Log("退出待机模式");
		}
	}
	public class PlayerRunState : AbstractState<Player.PlayerState, Player>
	{
		public PlayerRunState(FSM<Player.PlayerState> fsm, Player target) : base(fsm, target)
		{

		}
		protected override bool OnCondition()
		{
			return base.OnCondition();
		}
		protected override void OnEnter()
		{
			base.OnEnter();
			//Debug.Log("进入跑步状态");
		}
		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (Input.GetKeyDown(KeyCode.S))
			{
				mTarget.playerFSM.ChangeState(Player.PlayerState.下蹲);
			}

			if (Input.GetKeyDown(KeyCode.J))
			{
				mTarget.playerFSM.ChangeState(Player.PlayerState.攻击);
			}
		}
		protected override void OnFixedUpdate()
		{
			base.OnFixedUpdate();
			float runX = Input.GetAxisRaw("Horizontal");
			float runY = Input.GetAxisRaw("Vertical");
			//横向方向没速度说明玩家不处于跑步
			if (runX == 0)
			{
				mTarget.mRigidBody.velocity = Vector2.zero;
				mTarget.playerFSM.ChangeState(Player.PlayerState.待机);
			}
			else if (runY>0&&runX!=0)
			{
				//给刚体施加一个向上的瞬时力；预备进入跳跃
				mTarget.mRigidBody.AddForce(new Vector2(0,mTarget.playerModel.jumpForceValue),ForceMode2D.Impulse);
				Debug.Log("从跑步到跳跃");
				mFSM.ChangeState(Player.PlayerState.跳跃);
			}
			else
				mTarget.Run(runX, runY);
		}
		protected override void OnExit()
		{
			base.OnExit();
			//Debug.Log("退出跑步状态");
		}
	}
	public class PlayerCrouchState : AbstractState<Player.PlayerState, Player>
	{
		public PlayerCrouchState(FSM<Player.PlayerState> fsm, Player target) : base(fsm, target)
		{
		}
		protected override void OnEnter()
		{
			base.OnEnter();
			mTarget.playerAnimator.SetBool("isCrouch", true);
		}
		protected override void OnUpdate()
		{
			if (Input.GetKeyUp(KeyCode.S))
			{
				mFSM.ChangeState(Player.PlayerState.待机);
			}
		}
		protected override void OnFixedUpdate()
		{
			base.OnFixedUpdate();
			mTarget.mRigidBody.velocity = Vector2.zero;
		}
		protected override void OnExit()
		{
			base.OnExit();
			mTarget.playerAnimator.SetBool("isCrouch", false);
		}
	}
	public class PlayerJumpState : AbstractState<Player.PlayerState, Player>
	{
		public PlayerJumpState(FSM<Player.PlayerState> fsm, Player target) : base(fsm, target)
		{
		}

		protected override void OnEnter()
		{
			Debug.Log("进入跳跃状态");
			Debug.Log(mTarget.mRigidBody.velocity);
			mTarget.playerAnimator.SetBool("isOnPlatform",false);
		}

		protected override void OnUpdate()
		{
			//如果判定在平台上；并且Y轴方向没有速度；那说明玩家不处于跳跃的过程；切换到Idle
			if (mTarget.GetModel<PlayerModel>().isOnPlatform.Value&&mTarget.mRigidBody.velocity.y==0)
			{
				mFSM.ChangeState(Player.PlayerState.待机);
			}
			//如果判定在平台上；并且Y轴方向有负方向速度；那说明玩家处于落地瞬间；切换到Idle
			if (mTarget.GetModel<PlayerModel>().isOnPlatform.Value&&mTarget.mRigidBody.velocity.y<0f)
			{
				mFSM.ChangeState(Player.PlayerState.待机);
			}
		}

		protected override void OnFixedUpdate()
		{
			
		}

		protected override void OnExit()
		{
			mTarget.playerAnimator.SetBool("isOnPlatform",mTarget.playerModel.isOnPlatform.Value);
		}
	}
	public class PlayerAttack:AbstractState<Player.PlayerState, Player>
	{
		public PlayerAttack(FSM<Player.PlayerState> fsm, Player target) : base(fsm, target)
		{
		}

		protected override void OnEnter()
		{
			//播放第一段攻击动画
			mTarget.mRigidBody.velocity = Vector2.zero;
			mTarget.playerModel.isAttack.Value = true;
			mTarget.playerAnimator.SetBool("isAttack",true);
		}

		protected override void OnUpdate()
		{
			if (!mTarget.playerModel.isAttack.Value)
			{
				mFSM.ChangeState(Player.PlayerState.待机);
			}
		}

		protected override void OnFixedUpdate()
		{
			
		}

		protected override void OnExit()
		{
			mTarget.playerAnimator.SetBool("isAttack",false);
			mTarget.playerModel.isAttack.Value = false;
		}
	}

	#endregion

}
