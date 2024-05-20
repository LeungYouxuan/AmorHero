using System;
using System.Collections;
using UnityEngine;
using QFramework;
using QFramework.BuffDesign;
using QFramework.IModelScripts;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.AmorHero
{
	public partial class BuffController : ViewController,IController
	{
		private BuffModel buffModel;
		void Start()
		{
			// Code Here
			buffModel = this.GetModel<BuffModel>();
			this.SendCommand(new UpdateBuffEffect(this));
		}
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				//点击鼠标左键给玩家附加乌龟Buff
				this.SendCommand(new PlayerGetBuff(1));
			}
			if (Input.GetKeyDown(KeyCode.Mouse1))
			{
				//点击鼠标右键移除玩家身上的乌龟Buff
				this.SendCommand(new PlayerRemoveBuff(1));
			}
		}


		public IArchitecture GetArchitecture()
		{
			return AmorHeroArchitecture.Interface;
		}
	}
	public class UpdateBuffEffect:AbstractCommand
	{
		private BuffController buffController;

		public UpdateBuffEffect(BuffController buffController)
		{
			this.buffController = buffController;
		}
		IEnumerator CheckBuffEffect()
		{
			while (true)
			{
				var buffList = this.GetModel<BuffModel>().BuffList;
				if (buffList.Count != 0)
				{
					int i = 0;
					buffList[i].OnUpdate();
					i++;
					i = i % buffList.Count;
					yield return null;
				}
				yield return null;
			}
		}
		protected override void OnExecute()
		{
			buffController.StartCoroutine(CheckBuffEffect());
		}
	}

	public class PlayerGetBuff : AbstractCommand
	{
		private int buffID;
		public PlayerGetBuff(int buffID)
		{
			this.buffID = buffID;
		}
		protected override void OnExecute()
		{
			var model = this.GetModel<BuffModel>();
			if (model.BuffDict.ContainsKey(buffID))
			{
				model.BuffList.Add(model.BuffDict[buffID]);
				model.BuffDict[buffID].OnCreate();
			}
		}
	}

	public class PlayerRemoveBuff : AbstractCommand
	{
		private int buffID;

		public PlayerRemoveBuff(int buffID)
		{
			this.buffID = buffID;
		}
		protected override void OnExecute()
		{
			var model=this.GetModel<BuffModel>();
			for (int i = 0; i < model.BuffList.Count; i++)
			{
				if (buffID == model.BuffList[i].buff_id)
				{
					model.BuffList[i].OnDestroy();
					model.BuffList.RemoveAt(i);
				}
			}
		}
	}
}
