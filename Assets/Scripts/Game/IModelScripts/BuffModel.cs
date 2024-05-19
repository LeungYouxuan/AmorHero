using System.Collections;
using System.Collections.Generic;
using QFramework.BuffDesign;
using UnityEngine;

namespace QFramework.IModelScripts
{
    public class BuffModel : AbstractModel
    {
        public List<AbstractBuff> BuffList = new List<AbstractBuff>();
        public Dictionary<int,AbstractBuff> BuffDict = new Dictionary<int,AbstractBuff>();
        protected override void OnInit()
        {
            BuffDict.Add(1,new TurtlesBuff(1,"乌龟Buff"));
            Debug.Log("BuffModel被初始化");
        }
        
    }
}
