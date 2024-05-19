using UnityEngine;

namespace QFramework.AmorHero.ISystemScripts
{
    public class BuffSystem:AbstractSystem
    {

        protected override void OnInit()
        {
            Debug.Log("BuffSystem被初始化");
            this.RegisterEvent<TurtlesBuffEffect>(e =>
            {
                if (this.GetModel<PlayerModel>().playerHealth.Value <= 100)
                {
                    Debug.Log("触发乌龟Buff的特殊效果");
                }
            });
        }
        
    }

    public struct TurtlesBuffEffect
    {
    }
}