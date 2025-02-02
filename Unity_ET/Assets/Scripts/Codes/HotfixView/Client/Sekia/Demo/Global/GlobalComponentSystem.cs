using UnityEngine;
//Edit by Sekia

namespace ET.Client
{
    [ObjectSystem]
    public class GlobalComponentAwakeSystem: AwakeSystem<GlobalComponent>
    {
        protected override void Awake(GlobalComponent self)
        {
            GlobalComponent.Instance = self;
            self.Global = GameObject.Find("/Global").transform;
            
            self.globalConfig = Resources.Load<FairyGUI_GlobalConfig>("FairyGUI_GlobalConfig");
            if(self.globalConfig == null)
                Debug.LogError("没加载到文件");
            
            //Camera
            self.Camera_Main = GameObject.Find("/Global/Camera_Main").GetComponent<Camera>();
            self.Camera_UI = GameObject.Find("/Global/Camera_UI").GetComponent<Camera>();
            
            //Root
            self.Root_Unit = GameObject.Find("/Global/Root_Unit").transform;
        }
    }
}