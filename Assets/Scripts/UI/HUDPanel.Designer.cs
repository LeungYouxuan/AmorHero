using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:9b7baed3-0129-4ba4-afbf-7a58fdadbdaf
	public partial class HUDPanel
	{
		public const string Name = "HUDPanel";
		
		[SerializeField]
		public UnityEngine.UI.Text RunCountText;
		
		private HUDPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			RunCountText = null;
			
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
