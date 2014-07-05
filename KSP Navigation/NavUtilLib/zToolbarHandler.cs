/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Toolbar;
using HSI;


namespace HSI
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class ToolbarController : MonoBehaviour
    {
        #region toolbar integration

        private HSIController fHSI = new HSIController();

        private IButton tbButton;

        public bool bEditorVisible = false;
        public bool bFirstRun = true;

        private void Btn_Controller()
        {
            tbButton = ToolbarManager.Instance.add("HSI/MLS", "tbButton");
            tbButton.TexturePath = "AdvSRB/tool_btn";
            tbButton.ToolTip = "Open/Close Horizontal Situation Indicator/Microwave Landing System";
            tbButton.Visibility = new GameScenesVisibility(GameScenes.FLIGHT);
            tbButton.OnClick += (e) => ToggleController();
        }

        internal void OnDestroy()
        {
            tbButton.Destroy();
        }

        #endregion

        public void ToggleController()
        {
            /*
            bEditorVisible = !bEditorVisible;

            tbButton.TexturePath = bEditorVisible ? "AdvSRB/tool_btnX" : "AdvSRB/tool_btn";

            if (bEditorVisible)
            {
                fHSI.Activate();
            }

            if (bFirstRun)
            {
                fHSI.InitialPos();

            }

            if (!bEditorVisible)
                fHSI.Deactiveate();

            bFirstRun = false;
            /end
        }

        public void Awake()
        {
            
        }
    }
}
*/