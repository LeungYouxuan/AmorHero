using System;
using QFramework.AmorHero;
using QFramework.AmorHero.ISystemScripts;
using UnityEngine;
using System.Collections.Generic;
namespace QFramework.BuffDesign
{
    public interface IAbstractBuff
    {
        void OnCreate();
        void OnDestroy();
        void OnUpdate();
    }
    [System.Serializable]
    public class AbstractBuff : IAbstractBuff,ICanSendCommand,ICanSendEvent
    {
        public int buff_id;
        public string buff_name;
        public string buff_des;
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
    public static class BuffFactory
    {
        private static Dictionary<int, Type> buffTypeMapping = new Dictionary<int, Type>();

        static BuffFactory()
        {
            // 在这里注册所有的 Buff 类型
            buffTypeMapping[1] = typeof(TurtlesBuff);
            // 添加其他 Buff 的映射
        }
        public static AbstractBuff CreateBuff(int buffID, string buffName)
        {
            if (buffTypeMapping.TryGetValue(buffID, out Type buffType))
            {
                // 创建对应类型的 Buff 实例
                AbstractBuff buff = (AbstractBuff)Activator.CreateInstance(buffType, new object[] { buffID, buffName });
                return buff;
            }
            else
            {
                Debug.Log("BUFF工厂无法按照原料生成对应的BUFF！");
                return null;
                //throw new ArgumentException($"No buff found with the specified ID: {buffID}");
            }
        }
    }

    public class TurtlesBuff : AbstractBuff
    {
        public float timer=0f;//计时器，用于统计Buff的条件效果时间
        public TurtlesBuff(int buffID, string buffName)
        {
            this.buff_id = buffID;
            this.buff_name = buffName;
        }
        public override void OnCreate()
        {
            this.SendCommand(new ChangePlayerHealth(50));
            this.GetArchitecture().GetSystem<BuffSystem>().RegisterEvent<TurtlesBuffEffect>(e =>
            {
                //玄武庇护BUFF描述
                //当玩家生命值低于自身最大生命值的1/2时，提供一个每5秒恢复2点体力的效果
                var playerModel = this.GetArchitecture().GetModel<PlayerModel>();
                if (playerModel.playerHealth.Value <= playerModel.playerMaxHealth.Value/2)
                {
                    
                    timer += Time.deltaTime;
                    if (timer >= 5f)
                    {
                        playerModel.playerHealth.Value += 2;
                        timer = 0;
                        Debug.Log("触发玄武庇护效果");
                    }
                }
                // if (this.GetArchitecture().GetModel<PlayerModel>().playerHealth.Value <= 100)
                // {
                //     Debug.Log("触发玄武庇护效果");
                // }
            });
            Debug.Log("获得玄武庇护Buff效果");
        }
        public override void OnUpdate()
        {
            this.SendEvent<TurtlesBuffEffect>();
        }
        public override void OnDestroy()
        {
            this.SendCommand(new ChangePlayerHealth(-50));
            Debug.Log("丢失玄武庇护Buff效果");
        }
    }
}
