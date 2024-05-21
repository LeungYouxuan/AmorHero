using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:a7a302a5-7620-4d2c-bcea-1890f236702c
	public partial class HUDPanel
	{
		public const string Name = "HUDPanel";
		
		[SerializeField]
		public UnityEngine.UI.Text PlayerHealthText;
		[SerializeField]
		public UnityEngine.UI.Text PlayerMagicPointText;
		[SerializeField]
		public UnityEngine.UI.Text PlayerLevelText;
		
		private HUDPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			PlayerHealthText = null;
			PlayerMagicPointText = null;
			PlayerLevelText = null;
			
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
