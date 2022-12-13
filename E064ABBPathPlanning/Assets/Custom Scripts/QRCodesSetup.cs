using System;
using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using Microsoft.MixedReality.QR;
using TMPro;


namespace QRTracking
{
    public class QRCodesSetup : MonoBehaviour
    {
        [Tooltip("Determines if the QR codes scanner should be automatically started.")]
        public bool AutoStartQRTracking = false;

        [Tooltip("Visualize the detected QRCodes in the 3d space.")]
        public bool VisualizeQRCodes = true;

        public bool toggleQRTrackerbool = false;

        public PointPicking dataSlate;
        
        QRCodesManager qrCodesManager = null;

        void Awake()
        {
            qrCodesManager = QRCodesManager.Instance;
            if (AutoStartQRTracking)
            {
                qrCodesManager.StartQRTracking();
            }
            if (VisualizeQRCodes)
            {
                gameObject.AddComponent(typeof(QRTracking.QRCodesVisualizer));
            }
        }

        void Start()
        {
            
        }

        void Update()
        {

        }

        public void toggleQRtracker()
        {
            toggleQRTrackerbool = !toggleQRTrackerbool;

            if (toggleQRTrackerbool)
            {
                qrCodesManager.StartQRTracking();
                dataSlate.qrTrackingLabel.text = "QR Tracking: Enabled";
            }
            else
            {
                qrCodesManager.StopQRTracking();
                dataSlate.qrTrackingLabel.text = "QR Tracking: Disabled";
            }
        }
    }
}
