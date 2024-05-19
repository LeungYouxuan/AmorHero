using System;
using UnityEngine;
using QFramework;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.AmorHero
{
    public class CounterGameTimeModel : AbstractModel
    {
        public BindableProperty<float> currentGameTime = new BindableProperty<float>();
        public float CurrentTime;
        protected override void OnInit()
        {
            currentGameTime.Value = Time.time;
            CurrentTime = Time.time;
            currentGameTime.Register(newValue =>
            {
                //Debug.Log("游戏已经运行："+currentGameTime.Value);
            });
            Debug.Log("游戏开始!");
        }
    }
    
    public partial class CounterSystem : ViewController, IController
    {
        private CounterGameTimeModel gameTimeModel;
        void Start()
        {
            var instance = CounterGameTimeManager.Instance;
            gameTimeModel = this.GetModel<CounterGameTimeModel>();
        }

        private void Update()
        {
            this.SendCommand<IncreaseGameTotalTime>();
        }

        public IArchitecture GetArchitecture()
        {
            return CounterGameTimeApp.Interface;
        }
    }

    public class CounterGameTimeManager : MonoSingleton<CounterGameTimeManager>
    {
        
    }
    public class CounterGameTimeSystem : AbstractSystem
    {
        protected override void OnInit()
        {
            var model = this.GetModel<CounterGameTimeModel>();
            this.RegisterEvent<GameTimeIncreaseEvent>(e =>
            {
                if (model.CurrentTime < 30f && model.CurrentTime > 15f)
                {
                    Debug.Log("游戏时间不多了！！");
                }
                if (model.CurrentTime == 30f)
                {
                    Debug.Log("游戏结束");
                }
            });
            //Debug.Log("CounterGameTimeSystemSystem被初始化");
        }
    }

    public class CounterGameTimeApp : Architecture<CounterGameTimeApp>
    {
        protected override void Init()
        {
            //注册Model
            this.RegisterModel(new CounterGameTimeModel());
            //注册System
            this.RegisterSystem(new CounterGameTimeSystem());
        }
    }

    public class IncreaseGameTotalTime : AbstractCommand
    {
        protected override void OnExecute()
        {
            var model=this.GetModel<CounterGameTimeModel>();
            float duration = Time.time - model.currentGameTime.Value;
            model.currentGameTime.Value += duration;
            model.CurrentTime += duration;
            this.SendEvent<GameTimeIncreaseEvent>();
        }
    }
    public struct GameTimeIncreaseEvent
    {
    }
}