using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:7663191f-f0ba-48b4-83e5-9f3fa7033b51
	public partial class HUDPanel
	{
		public const string Name = "HUDPanel";
		
		[SerializeField]
		public UnityEngine.UI.Text PlayerHealthText;
		[SerializeField]
		public UnityEngine.UI.Text PlayerLevelText;
		[SerializeField]
		public UnityEngine.UI.Image FirstProp;
		[SerializeField]
		public UnityEngine.UI.Image SecondProp;
		[SerializeField]
		public UnityEngine.UI.Image ThirdProp;
		[SerializeField]
		public UnityEngine.UI.Image FourthProp;
		
		private HUDPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			PlayerHealthText = null;
			PlayerLevelText = null;
			FirstProp = null;
			SecondProp = null;
			ThirdProp = null;
			FourthProp = null;
			
			mData = null;
		}
		
		public HUDPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		HUDPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new HUDPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
