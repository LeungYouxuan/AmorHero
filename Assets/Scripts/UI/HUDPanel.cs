using UnityEngine;
using UnityEngine.UI;
using QFramework;
using QFramework.AmorHero;

namespace QFramework.Example
{
	public class HUDPanelData : UIPanelData
	{
	}
	public partial class HUDPanel : UIPanel,IController
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as HUDPanelData ?? new HUDPanelData();
			this.RegisterEvent<PlayerLevelChangeEvent>(e =>
			{
				PlayerLevelText.text = "等级："+this.GetModel<PlayerModel>().playerLevel.Value.ToString();
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			this.RegisterEvent<PlayerHealthChangeEvent>(e =>
			{
				PlayerHealthText.text = "生命值：" + this.GetModel<PlayerModel>().playerHealth.Value;
			});
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			PlayerHealthText.text = "生命值："+this.GetModel<PlayerModel>().playerHealth.ToString();
			PlayerLevelText.text = "等级："+this.GetModel<PlayerModel>().playerLevel.ToString();
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}

		public IArchitecture GetArchitecture()
		{
			return AmorHeroArchitecture.Interface;
		}
	}
}
