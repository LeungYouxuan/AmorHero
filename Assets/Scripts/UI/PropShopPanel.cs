using UnityEngine;
using UnityEngine.UI;
using QFramework;
using QFramework.AmorHero;
using QFramework.IModelScripts;
using System.Collections.Generic;
namespace QFramework.Example
{
	public class RefreshBuyList : AbstractCommand
	{
		protected override void OnExecute()
		{
			var model =this.GetModel<PropModel>();
			List<PropItem> allPropItems = model.allPropData.propItemList;
			//Debug.Log("当前商店存货量："+allPropItems.Count);
			model.buyPropList.Clear();
			for (int i = 0; i < 4; i++)
			{
				model.buyPropList.Add(allPropItems[Random.Range(0,5)]);
				//Debug.Log("第"+(i+1)+"个物品:"+model.buyPropList[i].prop_name);
			}
			//Debug.Log("Model层：刷新道具商店可购买物品！");
			this.SendEvent<RefreshBuyListEvent>();
		}
	}

	public class ShowPropContent : AbstractCommand
	{
		public string propName;
		public string propDes;
		public ShowPropContent(string propName,string propDes)
		{
			this.propName = propName;
			this.propDes = propDes;
		}
		protected override void OnExecute()
		{
			this.SendEvent(new ShowPropContentEvent(propName,propDes));
		}
	}
	public struct RefreshBuyListEvent
	{
		
	}

	public struct ShowPropContentEvent
	{
		public ShowPropContentEvent(string propName,string propDes)
		{
			this.propName = propName;
			this.propDes = propDes;
		}
		public string propName;
		public string propDes;
	}
	public class PropShopPanelData : UIPanelData
	{
	}
	public partial class PropShopPanel : UIPanel,IController
	{
		private PropModel propModel;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as PropShopPanelData ?? new PropShopPanelData();
			propModel = this.GetModel<PropModel>();
			// please add init code here
			this.RegisterEvent<RefreshBuyListEvent>(e =>
			{
				Prop1.sprite = propModel.buyPropList[0].prop_sprite;
				Prop2.sprite = propModel.buyPropList[1].prop_sprite;
				Prop3.sprite = propModel.buyPropList[2].prop_sprite;
				Prop4.sprite = propModel.buyPropList[3].prop_sprite;
				Prop1.color = new Color(255, 255, 255, 255);
				Prop2.color = new Color(255, 255, 255, 255);
				Prop3.color = new Color(255, 255, 255, 255);
				Prop4.color = new Color(255, 255, 255, 255);
				Prop1.GetComponent<PropImage>().thisPropItem = propModel.buyPropList[0];
				Prop2.GetComponent<PropImage>().thisPropItem = propModel.buyPropList[1];
				Prop3.GetComponent<PropImage>().thisPropItem = propModel.buyPropList[2];
				Prop4.GetComponent<PropImage>().thisPropItem = propModel.buyPropList[3];
				//Debug.Log("表现层通过Event刷新道具商店可购买物品");
			});
			this.RegisterEvent<ShowPropContentEvent>(e =>
			{
				PropNameText.text = e.propName;
				PropDesText.text = e.propDes;
			});
		}
		protected override void OnOpen(IUIData uiData = null)
		{
			//每次打开商店都会随机刷新四件物品供给玩家购买
			this.SendCommand(new RefreshBuyList());
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
