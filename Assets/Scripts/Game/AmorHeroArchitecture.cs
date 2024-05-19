using QFramework.AmorHero.ISystemScripts;
using QFramework.IModelScripts;
using UnityEngine;

namespace QFramework.AmorHero
{
    public class AmorHeroArchitecture:Architecture<AmorHeroArchitecture>
    {
        protected override void Init()
        {
            #region model注册
            
            RegisterModel<PlayerModel>(new PlayerModel());
            RegisterModel(new PropModel());
            #endregion
            

            #region System注册

            RegisterSystem(new LevelSystem());
            RegisterSystem(new WeaponSystem());
            RegisterSystem(new PropSystem());
            #endregion
            Debug.Log("Architecture完成初始化！！");
        }
    }
}