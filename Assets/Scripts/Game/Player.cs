using UnityEngine;
using QFramework;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.AmorHero
{
	#region Command
	public class IncreaseRunTime : AbstractCommand
	{
		protected override void OnExecute()
		{
			float duration=Time.time-this.GetModel<PlayerModel>().currentTime;//跑步的那些帧数所经过的时间
			Debug.Log("帧消耗时间:"+duration);
			this.GetModel<PlayerModel>().playerRunTime += duration;
			this.SendEvent<PlayerRunTimeChangeEvent>();
		}
	}
    public class InCreaseGameTime : AbstractCommand
    {
        protected override void OnExecute()
        {
           float duration=Time.time-this.GetModel<PlayerModel>().currentTime;
		   this.GetModel<PlayerModel>().currentTime+=duration;
		   this.SendEvent<GameTimeChangeEvent>();
        }
    }
    #endregion
    #region Event
    public struct PlayerRunTimeChangeEvent
	{

	}
	public struct GameTimeChangeEvent
	{

	}
	#endregion
	public class PlayerModel : AbstractModel
	{
		public float playerRunTime;//玩家跑步时长
		public float currentTime;//游戏总时间
		protected override void OnInit()
		{
			playerRunTime = 0;
			currentTime=Time.time;
		}
	}
	public class PlayerApp : Architecture<PlayerApp>
	{
		protected override void Init()
		{
			this.RegisterModel(new PlayerModel());
		}
	}
	public partial class Player : ViewController, IController
	{
		public enum PlayerState
		{
			待机, 跑步,
		}
		public FSM<PlayerState> playerFSM = new FSM<PlayerState>();
		public Vector2 currentDir = new Vector2(1, 0);//主角默认朝向
		public float runSpeed;//玩家基础移动速度大小
		public Animator playerAnimator;//玩家动画控制器组件
		private PlayerModel playerModel;
		Rigidbody2D mRigidBody;
		void Start()
		{
			// Code Here
			playerModel = this.GetModel<PlayerModel>();
			mRigidBody = GetComponent<Rigidbody2D>();
			playerAnimator = GetComponent<Animator>();
			#region 状态
			playerFSM.State(PlayerState.待机)
			.OnCondition(() => true)
			.OnEnter(() =>
			{
				Debug.Log("进入待机模式");
				gameObject.transform.localScale = new Vector3(currentDir.x, 1, 0);
				playerAnimator.SetFloat("runSpeed", 0f);
				mRigidBody.velocity = Vector2.zero;
			})
			.OnUpdate(() =>
			{
				//监听玩家控制
				float runX = Input.GetAxisRaw("Horizontal");
				float runY = Input.GetAxisRaw("Vertical");
				gameObject.transform.localScale = new Vector3(currentDir.x, 1, 1);
				//只有横向移动，说明玩家是跑步
				if (runX != 0 && runY == 0)
				{
					playerFSM.ChangeState(PlayerState.跑步);
				}
			})
			.OnExit(() =>
			{
				Debug.Log("退出待机模式");
			})
			;
			playerFSM.State(PlayerState.跑步)
			.OnCondition(() => true)
			.OnEnter(() =>
			{
				Debug.Log("进入跑步状态");
			})
			.OnUpdate(() =>
			{
				this.SendCommand<IncreaseRunTime>();
			})
			.OnFixedUpdate(() =>
			{

				float runX = Input.GetAxisRaw("Horizontal");
				float runY = Input.GetAxisRaw("Vertical");
				//横向方向没速度说明玩家不处于跑步
				if (runX == 0)
				{
					mRigidBody.velocity = Vector2.zero;
					playerFSM.ChangeState(PlayerState.待机);
				}
				else
					Run(runX, runY);
			})
			.OnExit(() =>
			{
				Debug.Log("退出跑步状态");
			});
			#endregion
			playerFSM.StartState(PlayerState.待机);
			this.RegisterEvent<PlayerRunTimeChangeEvent>(e =>
			{
				Debug.Log("跑步时长：" + playerModel.playerRunTime);
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			this.RegisterEvent<GameTimeChangeEvent>(e=>
			{
				if(Mathf.Approximately(playerModel.currentTime,10f))
				{
					Debug.Log("到达10秒!");
				}
				else if(playerModel.currentTime>10f)
				{

				}
				else
				{
					Debug.Log("游戏运行时间："+playerModel.currentTime);
				}
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
		}
		void Update()
		{
			playerFSM.Update();
			this.SendCommand<InCreaseGameTime>();
		}
		void FixedUpdate()
		{
			playerFSM.FixedUpdate();
		}
		void OnDestroy()
		{
			playerFSM.Clear();
		}
		void Run(float runX, float runY)
		{
			gameObject.transform.localScale = new Vector3(runX, 1, 0);
			mRigidBody.velocity = new Vector2(runX, runY) * runSpeed;
			playerAnimator.SetFloat("runSpeed", mRigidBody.velocity.sqrMagnitude);
			currentDir = new Vector2(runX, 0);
		}

		public IArchitecture GetArchitecture()
		{
			return PlayerApp.Interface;
		}
	}
}
