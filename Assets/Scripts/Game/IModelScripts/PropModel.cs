using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace QFramework.IModelScripts
{
    public class PropModel:AbstractModel
    {
        public PropData allPropData;
        private string propDataPath="Assets/Resource/Prop.asset";
        public List<PropItem> buyPropList = new List<PropItem>();
        protected override void OnInit()
        {
            AsyncOperationHandle<PropData> handle=Addressables.LoadAssetAsync<PropData>(propDataPath);
            handle.Completed += OnPropDataLoadedCompleted;
            Debug.Log("初始化PropModel");
        }

        void OnPropDataLoadedCompleted(AsyncOperationHandle<PropData> handle)
        {
            allPropData = handle.Result;
            Debug.Log("彻底初始化PropModel");
            Debug.Log("当前一共有："+allPropData.propItemList.Count+"个道具");
        }
    }

}
