using UnityEngine;

namespace QFramework.AmorHero.ISystemScripts
{
    public class LevelSystem:AbstractSystem
    {
        protected override void OnInit()
        {
            var model=this.GetModel<PlayerModel>();
            this.RegisterEvent<PlayerLevelChangeEvent>(e =>
            {
                
                if (model.playerLevel.Value <= 10)
                {
                    Debug.Log("玩家等级为："+model.playerLevel+"评价为初级");
                }
                else
                {
                    Debug.Log("玩家等级为："+model.playerLevel+"评价为高级");
                }
            });
            Debug.Log("初始化LevelSystem");
            this.SendEvent<PlayerLevelChangeEvent>();
        }
    }
}