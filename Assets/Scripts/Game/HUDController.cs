using System;
using UnityEngine;
using QFramework;
using QFramework.Example;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.AmorHero
{
	public partial class HUDController : ViewController
	{
		void Start()
		{
			// Code Here
			//打开HUD
			UIKit.OpenPanel<HUDPanel>();
			UIKit.Root.SetResolution(1920,1080,0.5f);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				UIKit.OpenPanel<PropShopPanel>();
				UIKit.Root.SetResolution(1920,1080,0.5f);
			}
		}
	}
}
