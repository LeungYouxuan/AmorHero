using UnityEngine;

namespace QFramework.AmorHero
{
    public class AmorHeroArchitecture:Architecture<AmorHeroArchitecture>
    {
        protected override void Init()
        {
            #region model注册

            RegisterModel<PlayerModel>(new PlayerModel());

            #endregion
            Debug.Log("Architecture完成初始化！！");
        }
    }
}