using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:72be9857-7502-4745-b18d-893b65b7b276
	public partial class PropShopPanel
	{
		public const string Name = "PropShopPanel";
		
		[SerializeField]
		public UnityEngine.UI.Image Prop1;
		[SerializeField]
		public UnityEngine.UI.Image Prop2;
		[SerializeField]
		public UnityEngine.UI.Image Prop3;
		[SerializeField]
		public UnityEngine.UI.Image Prop4;
		[SerializeField]
		public UnityEngine.UI.Text PropNameText;
		[SerializeField]
		public UnityEngine.UI.Text PropPropertyText;
		[SerializeField]
		public UnityEngine.UI.Text PropDesText;
		
		private PropShopPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Prop1 = null;
			Prop2 = null;
			Prop3 = null;
			Prop4 = null;
			PropNameText = null;
			PropPropertyText = null;
			PropDesText = null;
			
			mData = null;
		}
		
		public PropShopPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		PropShopPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new PropShopPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
