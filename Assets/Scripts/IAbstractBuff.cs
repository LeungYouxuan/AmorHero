using QFramework.AmorHero;
using QFramework.AmorHero.ISystemScripts;
using UnityEngine;

namespace QFramework.BuffDesign
{
    public interface IAbstractBuff
    {
        void OnCreate();
        void OnDestroy();
        void OnUpdate();
    }

    public abstract class AbstractBuff : IAbstractBuff,ICanSendCommand,ICanSendEvent
    {
        public int buffID;
        public string buffName;
        public virtual void OnCreate()
        {
            
        }

        public virtual void OnDestroy()
        {
            
        }

        public virtual void OnUpdate()
        {
            
        }
        public IArchitecture GetArchitecture()
        {
            return AmorHeroArchitecture.Interface;
        }
    }
    #region 基础Command

    public class RegisterBuffEffectToSystem : AbstractCommand
    {
        
        protected override void OnExecute()
        {
            var system=this.GetSystem<BuffSystem>();
            
        }
    }
    public class ChangePlayerHealth : AbstractCommand
    {
        private int changeValue;

        public ChangePlayerHealth(int changeValue)
        {
            this.changeValue = changeValue;
        }
        protected override void OnExecute()
        {
            this.GetModel<PlayerModel>().playerHealth.Value += changeValue;
            this.SendEvent<PlayerHealthChangeEvent>();
        }
    }

    public class ChangePlayerHealthByPercentage : AbstractCommand
    {
        private float changeValue;

        public ChangePlayerHealthByPercentage(int changeValue)
        {
            this.changeValue = changeValue;
        }
        protected override void OnExecute()
        {
            var model = this.GetModel<PlayerModel>().playerHealth;
            var resultValue = model.Value * changeValue + model.Value;
            model.Value = (int)resultValue;
            this.SendEvent<PlayerHealthChangeEvent>();
        }
    }
    #endregion
    public class TurtlesBuff : AbstractBuff
    {
        public TurtlesBuff(int buffID, string buffName)
        {
            this.buffID = buffID;
            this.buffName = buffName;
        }
        public override void OnCreate()
        {
            this.SendCommand(new ChangePlayerHealth(50));
            this.SendEvent<PlayerHealthChangeEvent>();
            Debug.Log("获得乌龟Buff效果");
        }
        public override void OnUpdate()
        {
            this.SendEvent<TurtlesBuffEffect>();
        }
        public override void OnDestroy()
        {
            this.SendCommand(new ChangePlayerHealth(-50));
            Debug.Log("丢失乌龟Buff效果");
        }
    }
}