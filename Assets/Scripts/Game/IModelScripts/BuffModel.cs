using System.Collections;
using System.Collections.Generic;
using QFramework.BuffDesign;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace QFramework.IModelScripts
{
    public class BuffModel : AbstractModel
    {
        public List<AbstractBuff> BuffList = new List<AbstractBuff>();
        public Dictionary<int,AbstractBuff> BuffDict = new Dictionary<int,AbstractBuff>();
        private string buffDataPath = "Assets/Resource/Buffs.asset";
        public BuffData allBuffData;
        protected override void OnInit()
        {
            AsyncOperationHandle<BuffData> handle = Addressables.LoadAssetAsync<BuffData>(buffDataPath);
            handle.Completed += OnBuffDataLoadedCompleted;
            Debug.Log("BuffModel被初始化");
        }
        void OnBuffDataLoadedCompleted(AsyncOperationHandle<BuffData> handle)
        {
            allBuffData = handle.Result;
            foreach (var VARIABLE in allBuffData.BuffList)
            {
                //这里要对每一个Buff进行装配成对应的Buff
                var buff=BuffFactory.CreateBuff(VARIABLE.buff_id, VARIABLE.buff_name);
                BuffDict.Add(VARIABLE.buff_id,buff);
            }
        }
    }
}
