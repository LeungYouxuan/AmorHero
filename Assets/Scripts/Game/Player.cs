using System;
using UnityEngine;
using QFramework;

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
	#endregion
	#region Event

	public struct PlayerJumpEvent
	{
		
	}
	#endregion
	public class PlayerModel : AbstractModel
	{
		public float jumpForceValue;//向上跳跃时施加力的大小
		public BindableProperty<int> playerHealth = new BindableProperty<int>();//玩家生命值上限
		public BindableProperty<float> playerRunSpeedValue=new BindableProperty<float>();//玩家移动速度大小
		public BindableProperty<bool> isOnPlatform=new BindableProperty<bool>();
		protected override void OnInit()
		{
			playerRunSpeedValue.Value =6f ;
			jumpForceValue = 8f;
			playerHealth.Value = 20;
			isOnPlatform.Value = true;
		}
	}

	public partial class Player : ViewController, IController
	{
		public enum PlayerState
		{
			待机, 跑步, 下蹲, 跳跃
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
			#endregion
			playerFSM.StartState(PlayerState.待机);
			#region 事件
			this.RegisterEvent<PlayerJumpEvent>(e =>
			{
				mRigidBody.velocity = new Vector2(mRigidBody.velocity.x, 0);
				Debug.Log("触发跳跃事件");
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
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
		public void Jump(float x,float y)
		{
			gameObject.transform.localScale = new Vector3(x, 1, 0);
			mRigidBody.velocity = new Vector2(x, y) * playerModel.playerRunSpeedValue.Value;
			currentDir = new Vector2(x, 0);
		}
		public IArchitecture GetArchitecture()
		{
			return AmorHeroArchitecture.Interface;
		}
	}
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
			Debug.Log("进入待机模式");
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
				mTarget.playerFSM.ChangeState(Player.PlayerState.跳跃);
			}
		}
		protected override void OnFixedUpdate()
		{
			base.OnFixedUpdate();
		}
		protected override void OnExit()
		{
			base.OnExit();
			Debug.Log("退出待机模式");
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
			Debug.Log("进入跑步状态");
		}
		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (Input.GetKeyDown(KeyCode.S))
			{
				mTarget.playerFSM.ChangeState(Player.PlayerState.下蹲);
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
				//保留速度
				mTarget.mRigidBody.velocity = new Vector2(runX*mTarget.playerModel.playerRunSpeedValue.Value, 0);
				mFSM.ChangeState(Player.PlayerState.跳跃);
			}
			else
				mTarget.Run(runX, runY);
		}
		protected override void OnExit()
		{
			base.OnExit();
			Debug.Log("退出跑步状态");
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
			mTarget.playerAnimator.SetBool("isOnPlatform",mTarget.playerModel.isOnPlatform.Value);
			Debug.Log(mTarget.mRigidBody.velocity);
			//给刚体施加一个向上的瞬时力
			mTarget.mRigidBody.AddForce(new Vector2(0,mTarget.playerModel.jumpForceValue),ForceMode2D.Impulse);

		}

		protected override void OnUpdate()
		{
			//检测玩家的isOnPlatform是否为true,并且Y轴上的速度为非正数
			if (mTarget.GetModel<PlayerModel>().isOnPlatform.Value)
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
}
