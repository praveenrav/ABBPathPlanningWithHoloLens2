using System;

namespace PCSDKApplication
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DataPanel = new System.Windows.Forms.Panel();
            this.pointCounterLabelValue = new System.Windows.Forms.Label();
            this.pointCounterLabel = new System.Windows.Forms.Label();
            this.sendDataButton = new System.Windows.Forms.Button();
            this.clearDataButton = new System.Windows.Forms.Button();
            this.goBackwardButton = new System.Windows.Forms.Button();
            this.goForwardButton = new System.Windows.Forms.Button();
            this.CPTBButton = new System.Windows.Forms.Button();
            this.CPTButton = new System.Windows.Forms.Button();
            this.ptpIndexLabel = new System.Windows.Forms.Label();
            this.rotListViewLabel = new System.Windows.Forms.Label();
            this.pathPlanningModeSectionLabel = new System.Windows.Forms.Label();
            this.zoneBox = new System.Windows.Forms.TextBox();
            this.posListViewLabel = new System.Windows.Forms.Label();
            this.ptpIndexSectionLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.xBoxPos = new System.Windows.Forms.TextBox();
            this.speedBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.pathViabilityLabelValue = new System.Windows.Forms.Label();
            this.savePointsButton = new System.Windows.Forms.Button();
            this.rotationListView = new System.Windows.Forms.ListView();
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.updateLabel = new System.Windows.Forms.Label();
            this.yBoxPos = new System.Windows.Forms.TextBox();
            this.positionListView = new System.Windows.Forms.ListView();
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.zBoxPos = new System.Windows.Forms.TextBox();
            this.pathViabilityLabel = new System.Windows.Forms.Label();
            this.disconnectContButton = new System.Windows.Forms.Button();
            this.stopRAPIDProgButton = new System.Windows.Forms.Button();
            this.startRapidProgButton = new System.Windows.Forms.Button();
            this.tcpButton = new System.Windows.Forms.Button();
            this.Progress1 = new System.Windows.Forms.Label();
            this.TCPPanel = new System.Windows.Forms.Panel();
            this.errMessLabel = new System.Windows.Forms.Label();
            this.errMessSectionLabel = new System.Windows.Forms.Label();
            this.servMessLabel = new System.Windows.Forms.Label();
            this.servMessSectionLabel = new System.Windows.Forms.Label();
            this.TCPWindowLabel = new System.Windows.Forms.Label();
            this.PortTextBox = new System.Windows.Forms.TextBox();
            this.PortLabel = new System.Windows.Forms.Label();
            this.IPAddrLabel = new System.Windows.Forms.Label();
            this.IPAddressTextBox = new System.Windows.Forms.TextBox();
            this.clientMessSectionLabel = new System.Windows.Forms.Label();
            this.clientMessLabel = new System.Windows.Forms.Label();
            this.ControllerPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.rapidProgramStatus2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.rapidProgramStatus1 = new System.Windows.Forms.Label();
            this.contrConnectionStatusLabel2 = new System.Windows.Forms.Label();
            this.contrConnectionStatus2 = new System.Windows.Forms.Label();
            this.contrConnectionStatusLabel1 = new System.Windows.Forms.Label();
            this.connectToControllerButton = new System.Windows.Forms.Button();
            this.RAPIDProgramStatusLabel = new System.Windows.Forms.Label();
            this.controllerWindowLabel = new System.Windows.Forms.Label();
            this.contrConnectionStatus1 = new System.Windows.Forms.Label();
            this.controllerConnectionStatusLabel = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.DataPanel.SuspendLayout();
            this.TCPPanel.SuspendLayout();
            this.ControllerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.listView1.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(12, 51);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(908, 146);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "IP Address";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "ID";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 125;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Availability";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 125;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Virtual";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "System Name";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader5.Width = 150;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "RW Version";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader6.Width = 150;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Controller Name";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader7.Width = 150;
            // 
            // DataPanel
            // 
            this.DataPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.DataPanel.Controls.Add(this.pointCounterLabelValue);
            this.DataPanel.Controls.Add(this.pointCounterLabel);
            this.DataPanel.Controls.Add(this.sendDataButton);
            this.DataPanel.Controls.Add(this.clearDataButton);
            this.DataPanel.Controls.Add(this.goBackwardButton);
            this.DataPanel.Controls.Add(this.goForwardButton);
            this.DataPanel.Controls.Add(this.CPTBButton);
            this.DataPanel.Controls.Add(this.CPTButton);
            this.DataPanel.Controls.Add(this.ptpIndexLabel);
            this.DataPanel.Controls.Add(this.rotListViewLabel);
            this.DataPanel.Controls.Add(this.pathPlanningModeSectionLabel);
            this.DataPanel.Controls.Add(this.zoneBox);
            this.DataPanel.Controls.Add(this.posListViewLabel);
            this.DataPanel.Controls.Add(this.ptpIndexSectionLabel);
            this.DataPanel.Controls.Add(this.label6);
            this.DataPanel.Controls.Add(this.xBoxPos);
            this.DataPanel.Controls.Add(this.speedBox);
            this.DataPanel.Controls.Add(this.label8);
            this.DataPanel.Controls.Add(this.pathViabilityLabelValue);
            this.DataPanel.Controls.Add(this.savePointsButton);
            this.DataPanel.Controls.Add(this.rotationListView);
            this.DataPanel.Controls.Add(this.updateLabel);
            this.DataPanel.Controls.Add(this.yBoxPos);
            this.DataPanel.Controls.Add(this.positionListView);
            this.DataPanel.Controls.Add(this.zBoxPos);
            this.DataPanel.Controls.Add(this.pathViabilityLabel);
            this.DataPanel.Location = new System.Drawing.Point(12, 540);
            this.DataPanel.Name = "DataPanel";
            this.DataPanel.Size = new System.Drawing.Size(1476, 372);
            this.DataPanel.TabIndex = 1;
            // 
            // pointCounterLabelValue
            // 
            this.pointCounterLabelValue.AutoSize = true;
            this.pointCounterLabelValue.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pointCounterLabelValue.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.pointCounterLabelValue.Location = new System.Drawing.Point(1284, 91);
            this.pointCounterLabelValue.Name = "pointCounterLabelValue";
            this.pointCounterLabelValue.Size = new System.Drawing.Size(24, 26);
            this.pointCounterLabelValue.TabIndex = 33;
            this.pointCounterLabelValue.Text = "0";
            this.pointCounterLabelValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pointCounterLabel
            // 
            this.pointCounterLabel.AutoSize = true;
            this.pointCounterLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pointCounterLabel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pointCounterLabel.Location = new System.Drawing.Point(1105, 91);
            this.pointCounterLabel.Name = "pointCounterLabel";
            this.pointCounterLabel.Size = new System.Drawing.Size(187, 26);
            this.pointCounterLabel.TabIndex = 32;
            this.pointCounterLabel.Text = "Number of Points:";
            this.pointCounterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sendDataButton
            // 
            this.sendDataButton.BackColor = System.Drawing.Color.Yellow;
            this.sendDataButton.Enabled = false;
            this.sendDataButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sendDataButton.Location = new System.Drawing.Point(1102, 257);
            this.sendDataButton.Name = "sendDataButton";
            this.sendDataButton.Size = new System.Drawing.Size(154, 88);
            this.sendDataButton.TabIndex = 31;
            this.sendDataButton.Text = "Send Data";
            this.sendDataButton.UseVisualStyleBackColor = false;
            this.sendDataButton.Click += new System.EventHandler(this.sendDataButton_Click);
            // 
            // clearDataButton
            // 
            this.clearDataButton.BackColor = System.Drawing.Color.Magenta;
            this.clearDataButton.Enabled = false;
            this.clearDataButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearDataButton.Location = new System.Drawing.Point(1289, 257);
            this.clearDataButton.Name = "clearDataButton";
            this.clearDataButton.Size = new System.Drawing.Size(154, 88);
            this.clearDataButton.TabIndex = 30;
            this.clearDataButton.Text = "Clear Data";
            this.clearDataButton.UseVisualStyleBackColor = false;
            this.clearDataButton.Click += new System.EventHandler(this.clearDataButton_Click);
            // 
            // goBackwardButton
            // 
            this.goBackwardButton.BackColor = System.Drawing.Color.Red;
            this.goBackwardButton.Enabled = false;
            this.goBackwardButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.goBackwardButton.Location = new System.Drawing.Point(1102, 196);
            this.goBackwardButton.Name = "goBackwardButton";
            this.goBackwardButton.Size = new System.Drawing.Size(154, 55);
            this.goBackwardButton.TabIndex = 29;
            this.goBackwardButton.Text = "Go Backward";
            this.goBackwardButton.UseVisualStyleBackColor = false;
            this.goBackwardButton.Click += new System.EventHandler(this.goBackwardButton_Click);
            // 
            // goForwardButton
            // 
            this.goForwardButton.BackColor = System.Drawing.Color.LimeGreen;
            this.goForwardButton.Enabled = false;
            this.goForwardButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.goForwardButton.Location = new System.Drawing.Point(1102, 135);
            this.goForwardButton.Name = "goForwardButton";
            this.goForwardButton.Size = new System.Drawing.Size(154, 55);
            this.goForwardButton.TabIndex = 28;
            this.goForwardButton.Text = "Go Forward";
            this.goForwardButton.UseVisualStyleBackColor = false;
            this.goForwardButton.Click += new System.EventHandler(this.goForwardButton_Click);
            // 
            // CPTBButton
            // 
            this.CPTBButton.BackColor = System.Drawing.Color.DarkOrange;
            this.CPTBButton.Enabled = false;
            this.CPTBButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CPTBButton.Location = new System.Drawing.Point(1289, 196);
            this.CPTBButton.Name = "CPTBButton";
            this.CPTBButton.Size = new System.Drawing.Size(154, 55);
            this.CPTBButton.TabIndex = 16;
            this.CPTBButton.Text = "Rewind";
            this.CPTBButton.UseVisualStyleBackColor = false;
            this.CPTBButton.Click += new System.EventHandler(this.CPTBButton_Click);
            // 
            // CPTButton
            // 
            this.CPTButton.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.CPTButton.Enabled = false;
            this.CPTButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CPTButton.Location = new System.Drawing.Point(1289, 135);
            this.CPTButton.Name = "CPTButton";
            this.CPTButton.Size = new System.Drawing.Size(154, 55);
            this.CPTButton.TabIndex = 19;
            this.CPTButton.Text = "Play";
            this.CPTButton.UseVisualStyleBackColor = false;
            this.CPTButton.Click += new System.EventHandler(this.CPTButton_Click);
            // 
            // ptpIndexLabel
            // 
            this.ptpIndexLabel.AutoSize = true;
            this.ptpIndexLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ptpIndexLabel.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.ptpIndexLabel.Location = new System.Drawing.Point(1213, 57);
            this.ptpIndexLabel.Name = "ptpIndexLabel";
            this.ptpIndexLabel.Size = new System.Drawing.Size(24, 26);
            this.ptpIndexLabel.TabIndex = 27;
            this.ptpIndexLabel.Text = "1";
            this.ptpIndexLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rotListViewLabel
            // 
            this.rotListViewLabel.AutoSize = true;
            this.rotListViewLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rotListViewLabel.ForeColor = System.Drawing.Color.Black;
            this.rotListViewLabel.Location = new System.Drawing.Point(345, 58);
            this.rotListViewLabel.Name = "rotListViewLabel";
            this.rotListViewLabel.Size = new System.Drawing.Size(132, 19);
            this.rotListViewLabel.TabIndex = 25;
            this.rotListViewLabel.Text = "Rotation Values:";
            // 
            // pathPlanningModeSectionLabel
            // 
            this.pathPlanningModeSectionLabel.AutoSize = true;
            this.pathPlanningModeSectionLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathPlanningModeSectionLabel.Location = new System.Drawing.Point(9, 12);
            this.pathPlanningModeSectionLabel.Name = "pathPlanningModeSectionLabel";
            this.pathPlanningModeSectionLabel.Size = new System.Drawing.Size(272, 26);
            this.pathPlanningModeSectionLabel.TabIndex = 18;
            this.pathPlanningModeSectionLabel.Text = "Path-Planning Information:";
            this.pathPlanningModeSectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // zoneBox
            // 
            this.zoneBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.zoneBox.Location = new System.Drawing.Point(868, 283);
            this.zoneBox.Name = "zoneBox";
            this.zoneBox.Size = new System.Drawing.Size(100, 32);
            this.zoneBox.TabIndex = 15;
            this.zoneBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // posListViewLabel
            // 
            this.posListViewLabel.AutoSize = true;
            this.posListViewLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posListViewLabel.ForeColor = System.Drawing.Color.Black;
            this.posListViewLabel.Location = new System.Drawing.Point(8, 58);
            this.posListViewLabel.Name = "posListViewLabel";
            this.posListViewLabel.Size = new System.Drawing.Size(174, 19);
            this.posListViewLabel.TabIndex = 24;
            this.posListViewLabel.Text = "Position Values [mm]:";
            // 
            // ptpIndexSectionLabel
            // 
            this.ptpIndexSectionLabel.AutoSize = true;
            this.ptpIndexSectionLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ptpIndexSectionLabel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ptpIndexSectionLabel.Location = new System.Drawing.Point(1105, 57);
            this.ptpIndexSectionLabel.Name = "ptpIndexSectionLabel";
            this.ptpIndexSectionLabel.Size = new System.Drawing.Size(112, 26);
            this.ptpIndexSectionLabel.TabIndex = 26;
            this.ptpIndexSectionLabel.Text = "PTP Index:";
            this.ptpIndexSectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(807, 286);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 26);
            this.label6.TabIndex = 14;
            this.label6.Text = "Zone:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // xBoxPos
            // 
            this.xBoxPos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xBoxPos.Location = new System.Drawing.Point(799, 83);
            this.xBoxPos.Name = "xBoxPos";
            this.xBoxPos.Size = new System.Drawing.Size(100, 22);
            this.xBoxPos.TabIndex = 2;
            // 
            // speedBox
            // 
            this.speedBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.speedBox.Location = new System.Drawing.Point(868, 240);
            this.speedBox.Name = "speedBox";
            this.speedBox.Size = new System.Drawing.Size(100, 32);
            this.speedBox.TabIndex = 11;
            this.speedBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(795, 243);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 26);
            this.label8.TabIndex = 13;
            this.label8.Text = "Speed:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pathViabilityLabelValue
            // 
            this.pathViabilityLabelValue.AutoSize = true;
            this.pathViabilityLabelValue.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathViabilityLabelValue.ForeColor = System.Drawing.Color.RoyalBlue;
            this.pathViabilityLabelValue.Location = new System.Drawing.Point(1246, 21);
            this.pathViabilityLabelValue.Name = "pathViabilityLabelValue";
            this.pathViabilityLabelValue.Size = new System.Drawing.Size(154, 26);
            this.pathViabilityLabelValue.TabIndex = 18;
            this.pathViabilityLabelValue.Text = "NOT CHECKED";
            this.pathViabilityLabelValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // savePointsButton
            // 
            this.savePointsButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.savePointsButton.Location = new System.Drawing.Point(905, 91);
            this.savePointsButton.Name = "savePointsButton";
            this.savePointsButton.Size = new System.Drawing.Size(133, 56);
            this.savePointsButton.TabIndex = 3;
            this.savePointsButton.Text = "Save Point";
            this.savePointsButton.UseVisualStyleBackColor = true;
            this.savePointsButton.Click += new System.EventHandler(this.savePointsButton_Click);
            // 
            // rotationListView
            // 
            this.rotationListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader14});
            this.rotationListView.FullRowSelect = true;
            this.rotationListView.GridLines = true;
            this.rotationListView.HideSelection = false;
            this.rotationListView.Location = new System.Drawing.Point(349, 80);
            this.rotationListView.Name = "rotationListView";
            this.rotationListView.Size = new System.Drawing.Size(405, 248);
            this.rotationListView.TabIndex = 19;
            this.rotationListView.UseCompatibleStateImageBehavior = false;
            this.rotationListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "W";
            this.columnHeader11.Width = 101;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "X";
            this.columnHeader12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader12.Width = 100;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Y";
            this.columnHeader13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader13.Width = 100;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "Z";
            this.columnHeader14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader14.Width = 100;
            // 
            // updateLabel
            // 
            this.updateLabel.AutoSize = true;
            this.updateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateLabel.Location = new System.Drawing.Point(202, 62);
            this.updateLabel.Name = "updateLabel";
            this.updateLabel.Size = new System.Drawing.Size(0, 25);
            this.updateLabel.TabIndex = 11;
            // 
            // yBoxPos
            // 
            this.yBoxPos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yBoxPos.Location = new System.Drawing.Point(799, 109);
            this.yBoxPos.Name = "yBoxPos";
            this.yBoxPos.Size = new System.Drawing.Size(100, 22);
            this.yBoxPos.TabIndex = 3;
            // 
            // positionListView
            // 
            this.positionListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10});
            this.positionListView.FullRowSelect = true;
            this.positionListView.GridLines = true;
            this.positionListView.HideSelection = false;
            this.positionListView.Location = new System.Drawing.Point(12, 80);
            this.positionListView.Name = "positionListView";
            this.positionListView.Size = new System.Drawing.Size(310, 248);
            this.positionListView.TabIndex = 7;
            this.positionListView.UseCompatibleStateImageBehavior = false;
            this.positionListView.View = System.Windows.Forms.View.Details;
            this.positionListView.DoubleClick += new System.EventHandler(this.positionListView_DoubleClick);
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "X";
            this.columnHeader8.Width = 101;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Y";
            this.columnHeader9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader9.Width = 100;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Z";
            this.columnHeader10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader10.Width = 100;
            // 
            // zBoxPos
            // 
            this.zBoxPos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.zBoxPos.Location = new System.Drawing.Point(799, 135);
            this.zBoxPos.Name = "zBoxPos";
            this.zBoxPos.Size = new System.Drawing.Size(100, 22);
            this.zBoxPos.TabIndex = 4;
            // 
            // pathViabilityLabel
            // 
            this.pathViabilityLabel.AutoSize = true;
            this.pathViabilityLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathViabilityLabel.Location = new System.Drawing.Point(1105, 21);
            this.pathViabilityLabel.Name = "pathViabilityLabel";
            this.pathViabilityLabel.Size = new System.Drawing.Size(147, 26);
            this.pathViabilityLabel.TabIndex = 17;
            this.pathViabilityLabel.Text = "Path Viability:";
            this.pathViabilityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // disconnectContButton
            // 
            this.disconnectContButton.BackColor = System.Drawing.Color.Red;
            this.disconnectContButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disconnectContButton.Location = new System.Drawing.Point(228, 203);
            this.disconnectContButton.Name = "disconnectContButton";
            this.disconnectContButton.Size = new System.Drawing.Size(184, 86);
            this.disconnectContButton.TabIndex = 2;
            this.disconnectContButton.Text = "Disconnect from Controller";
            this.disconnectContButton.UseVisualStyleBackColor = false;
            this.disconnectContButton.Click += new System.EventHandler(this.disconnectFromController);
            // 
            // stopRAPIDProgButton
            // 
            this.stopRAPIDProgButton.BackColor = System.Drawing.Color.Firebrick;
            this.stopRAPIDProgButton.Enabled = false;
            this.stopRAPIDProgButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopRAPIDProgButton.Location = new System.Drawing.Point(738, 203);
            this.stopRAPIDProgButton.Name = "stopRAPIDProgButton";
            this.stopRAPIDProgButton.Size = new System.Drawing.Size(182, 86);
            this.stopRAPIDProgButton.TabIndex = 1;
            this.stopRAPIDProgButton.Text = "Stop RAPID Program";
            this.stopRAPIDProgButton.UseVisualStyleBackColor = false;
            this.stopRAPIDProgButton.Click += new System.EventHandler(this.StopRapidProgram);
            // 
            // startRapidProgButton
            // 
            this.startRapidProgButton.BackColor = System.Drawing.Color.LimeGreen;
            this.startRapidProgButton.Enabled = false;
            this.startRapidProgButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startRapidProgButton.Location = new System.Drawing.Point(537, 203);
            this.startRapidProgButton.Name = "startRapidProgButton";
            this.startRapidProgButton.Size = new System.Drawing.Size(184, 86);
            this.startRapidProgButton.TabIndex = 0;
            this.startRapidProgButton.Text = "Start RAPID Program";
            this.startRapidProgButton.UseVisualStyleBackColor = false;
            this.startRapidProgButton.Click += new System.EventHandler(this.StartRapidProgram);
            // 
            // tcpButton
            // 
            this.tcpButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcpButton.Location = new System.Drawing.Point(14, 45);
            this.tcpButton.Name = "tcpButton";
            this.tcpButton.Size = new System.Drawing.Size(140, 79);
            this.tcpButton.TabIndex = 8;
            this.tcpButton.Text = "Start TCP Server";
            this.tcpButton.UseVisualStyleBackColor = true;
            this.tcpButton.Click += new System.EventHandler(this.tcpButton_Click);
            // 
            // Progress1
            // 
            this.Progress1.AutoSize = true;
            this.Progress1.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Progress1.Location = new System.Drawing.Point(42, 154);
            this.Progress1.Name = "Progress1";
            this.Progress1.Size = new System.Drawing.Size(270, 26);
            this.Progress1.TabIndex = 9;
            this.Progress1.Text = "Ready to connect to client!";
            // 
            // TCPPanel
            // 
            this.TCPPanel.BackColor = System.Drawing.Color.LightCyan;
            this.TCPPanel.Controls.Add(this.errMessLabel);
            this.TCPPanel.Controls.Add(this.errMessSectionLabel);
            this.TCPPanel.Controls.Add(this.servMessLabel);
            this.TCPPanel.Controls.Add(this.servMessSectionLabel);
            this.TCPPanel.Controls.Add(this.TCPWindowLabel);
            this.TCPPanel.Controls.Add(this.PortTextBox);
            this.TCPPanel.Controls.Add(this.PortLabel);
            this.TCPPanel.Controls.Add(this.IPAddrLabel);
            this.TCPPanel.Controls.Add(this.IPAddressTextBox);
            this.TCPPanel.Controls.Add(this.clientMessSectionLabel);
            this.TCPPanel.Controls.Add(this.clientMessLabel);
            this.TCPPanel.Controls.Add(this.tcpButton);
            this.TCPPanel.Controls.Add(this.Progress1);
            this.TCPPanel.Location = new System.Drawing.Point(12, 12);
            this.TCPPanel.Name = "TCPPanel";
            this.TCPPanel.Size = new System.Drawing.Size(1476, 211);
            this.TCPPanel.TabIndex = 10;
            // 
            // errMessLabel
            // 
            this.errMessLabel.AutoSize = true;
            this.errMessLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.errMessLabel.Location = new System.Drawing.Point(1075, 45);
            this.errMessLabel.Name = "errMessLabel";
            this.errMessLabel.Size = new System.Drawing.Size(162, 19);
            this.errMessLabel.TabIndex = 22;
            this.errMessLabel.Text = "Awaiting error message...";
            this.errMessLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // errMessSectionLabel
            // 
            this.errMessSectionLabel.AutoSize = true;
            this.errMessSectionLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.errMessSectionLabel.Location = new System.Drawing.Point(1074, 12);
            this.errMessSectionLabel.Name = "errMessSectionLabel";
            this.errMessSectionLabel.Size = new System.Drawing.Size(154, 26);
            this.errMessSectionLabel.TabIndex = 21;
            this.errMessSectionLabel.Text = "Error Message:";
            // 
            // servMessLabel
            // 
            this.servMessLabel.AutoSize = true;
            this.servMessLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.servMessLabel.Location = new System.Drawing.Point(478, 45);
            this.servMessLabel.Name = "servMessLabel";
            this.servMessLabel.Size = new System.Drawing.Size(169, 19);
            this.servMessLabel.TabIndex = 20;
            this.servMessLabel.Text = "Awaiting server message...";
            this.servMessLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // servMessSectionLabel
            // 
            this.servMessSectionLabel.AutoSize = true;
            this.servMessSectionLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.servMessSectionLabel.Location = new System.Drawing.Point(477, 12);
            this.servMessSectionLabel.Name = "servMessSectionLabel";
            this.servMessSectionLabel.Size = new System.Drawing.Size(167, 26);
            this.servMessSectionLabel.TabIndex = 19;
            this.servMessSectionLabel.Text = "Server Message:";
            // 
            // TCPWindowLabel
            // 
            this.TCPWindowLabel.AutoSize = true;
            this.TCPWindowLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TCPWindowLabel.Location = new System.Drawing.Point(9, 12);
            this.TCPWindowLabel.Name = "TCPWindowLabel";
            this.TCPWindowLabel.Size = new System.Drawing.Size(196, 26);
            this.TCPWindowLabel.TabIndex = 18;
            this.TCPWindowLabel.Text = "TCP Server Section:";
            this.TCPWindowLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PortTextBox
            // 
            this.PortTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PortTextBox.Location = new System.Drawing.Point(255, 86);
            this.PortTextBox.Name = "PortTextBox";
            this.PortTextBox.Size = new System.Drawing.Size(97, 24);
            this.PortTextBox.TabIndex = 17;
            this.PortTextBox.Text = "13";
            this.PortTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // PortLabel
            // 
            this.PortLabel.AutoSize = true;
            this.PortLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PortLabel.Location = new System.Drawing.Point(215, 89);
            this.PortLabel.Name = "PortLabel";
            this.PortLabel.Size = new System.Drawing.Size(38, 19);
            this.PortLabel.TabIndex = 16;
            this.PortLabel.Text = "Port:";
            // 
            // IPAddrLabel
            // 
            this.IPAddrLabel.AutoSize = true;
            this.IPAddrLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IPAddrLabel.Location = new System.Drawing.Point(173, 63);
            this.IPAddrLabel.Name = "IPAddrLabel";
            this.IPAddrLabel.Size = new System.Drawing.Size(77, 19);
            this.IPAddrLabel.TabIndex = 15;
            this.IPAddrLabel.Text = "IP Address:";
            // 
            // IPAddressTextBox
            // 
            this.IPAddressTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IPAddressTextBox.Location = new System.Drawing.Point(255, 60);
            this.IPAddressTextBox.Name = "IPAddressTextBox";
            this.IPAddressTextBox.Size = new System.Drawing.Size(97, 24);
            this.IPAddressTextBox.TabIndex = 14;
            this.IPAddressTextBox.Text = "127.0.0.1";
            this.IPAddressTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // clientMessSectionLabel
            // 
            this.clientMessSectionLabel.AutoSize = true;
            this.clientMessSectionLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clientMessSectionLabel.Location = new System.Drawing.Point(770, 12);
            this.clientMessSectionLabel.Name = "clientMessSectionLabel";
            this.clientMessSectionLabel.Size = new System.Drawing.Size(163, 26);
            this.clientMessSectionLabel.TabIndex = 13;
            this.clientMessSectionLabel.Text = "Client Message:";
            // 
            // clientMessLabel
            // 
            this.clientMessLabel.AutoSize = true;
            this.clientMessLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clientMessLabel.Location = new System.Drawing.Point(771, 45);
            this.clientMessLabel.MaximumSize = new System.Drawing.Size(300, 300);
            this.clientMessLabel.Name = "clientMessLabel";
            this.clientMessLabel.Size = new System.Drawing.Size(165, 19);
            this.clientMessLabel.TabIndex = 10;
            this.clientMessLabel.Text = "Awaiting client message...";
            this.clientMessLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ControllerPanel
            // 
            this.ControllerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ControllerPanel.Controls.Add(this.label1);
            this.ControllerPanel.Controls.Add(this.rapidProgramStatus2);
            this.ControllerPanel.Controls.Add(this.label3);
            this.ControllerPanel.Controls.Add(this.rapidProgramStatus1);
            this.ControllerPanel.Controls.Add(this.contrConnectionStatusLabel2);
            this.ControllerPanel.Controls.Add(this.contrConnectionStatus2);
            this.ControllerPanel.Controls.Add(this.contrConnectionStatusLabel1);
            this.ControllerPanel.Controls.Add(this.connectToControllerButton);
            this.ControllerPanel.Controls.Add(this.RAPIDProgramStatusLabel);
            this.ControllerPanel.Controls.Add(this.controllerWindowLabel);
            this.ControllerPanel.Controls.Add(this.contrConnectionStatus1);
            this.ControllerPanel.Controls.Add(this.controllerConnectionStatusLabel);
            this.ControllerPanel.Controls.Add(this.listView1);
            this.ControllerPanel.Controls.Add(this.stopRAPIDProgButton);
            this.ControllerPanel.Controls.Add(this.disconnectContButton);
            this.ControllerPanel.Controls.Add(this.startRapidProgButton);
            this.ControllerPanel.Location = new System.Drawing.Point(12, 229);
            this.ControllerPanel.Name = "ControllerPanel";
            this.ControllerPanel.Size = new System.Drawing.Size(1476, 305);
            this.ControllerPanel.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Purple;
            this.label1.Location = new System.Drawing.Point(938, 217);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 19);
            this.label1.TabIndex = 29;
            this.label1.Text = "Controller 2:";
            // 
            // rapidProgramStatus2
            // 
            this.rapidProgramStatus2.AutoSize = true;
            this.rapidProgramStatus2.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rapidProgramStatus2.ForeColor = System.Drawing.Color.IndianRed;
            this.rapidProgramStatus2.Location = new System.Drawing.Point(1035, 217);
            this.rapidProgramStatus2.Name = "rapidProgramStatus2";
            this.rapidProgramStatus2.Size = new System.Drawing.Size(102, 19);
            this.rapidProgramStatus2.TabIndex = 28;
            this.rapidProgramStatus2.Text = "Not running";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Purple;
            this.label3.Location = new System.Drawing.Point(938, 188);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 19);
            this.label3.TabIndex = 27;
            this.label3.Text = "Controller 1:";
            // 
            // rapidProgramStatus1
            // 
            this.rapidProgramStatus1.AutoSize = true;
            this.rapidProgramStatus1.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rapidProgramStatus1.ForeColor = System.Drawing.Color.IndianRed;
            this.rapidProgramStatus1.Location = new System.Drawing.Point(1035, 188);
            this.rapidProgramStatus1.Name = "rapidProgramStatus1";
            this.rapidProgramStatus1.Size = new System.Drawing.Size(102, 19);
            this.rapidProgramStatus1.TabIndex = 26;
            this.rapidProgramStatus1.Text = "Not running";
            // 
            // contrConnectionStatusLabel2
            // 
            this.contrConnectionStatusLabel2.AutoSize = true;
            this.contrConnectionStatusLabel2.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contrConnectionStatusLabel2.ForeColor = System.Drawing.Color.Purple;
            this.contrConnectionStatusLabel2.Location = new System.Drawing.Point(938, 104);
            this.contrConnectionStatusLabel2.Name = "contrConnectionStatusLabel2";
            this.contrConnectionStatusLabel2.Size = new System.Drawing.Size(101, 19);
            this.contrConnectionStatusLabel2.TabIndex = 25;
            this.contrConnectionStatusLabel2.Text = "Controller 2:";
            // 
            // contrConnectionStatus2
            // 
            this.contrConnectionStatus2.AutoSize = true;
            this.contrConnectionStatus2.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contrConnectionStatus2.ForeColor = System.Drawing.Color.IndianRed;
            this.contrConnectionStatus2.Location = new System.Drawing.Point(1035, 104);
            this.contrConnectionStatus2.Name = "contrConnectionStatus2";
            this.contrConnectionStatus2.Size = new System.Drawing.Size(122, 19);
            this.contrConnectionStatus2.TabIndex = 24;
            this.contrConnectionStatus2.Text = "Not connected";
            // 
            // contrConnectionStatusLabel1
            // 
            this.contrConnectionStatusLabel1.AutoSize = true;
            this.contrConnectionStatusLabel1.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contrConnectionStatusLabel1.ForeColor = System.Drawing.Color.Purple;
            this.contrConnectionStatusLabel1.Location = new System.Drawing.Point(938, 75);
            this.contrConnectionStatusLabel1.Name = "contrConnectionStatusLabel1";
            this.contrConnectionStatusLabel1.Size = new System.Drawing.Size(101, 19);
            this.contrConnectionStatusLabel1.TabIndex = 23;
            this.contrConnectionStatusLabel1.Text = "Controller 1:";
            // 
            // connectToControllerButton
            // 
            this.connectToControllerButton.BackColor = System.Drawing.Color.Chartreuse;
            this.connectToControllerButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectToControllerButton.Location = new System.Drawing.Point(14, 203);
            this.connectToControllerButton.Name = "connectToControllerButton";
            this.connectToControllerButton.Size = new System.Drawing.Size(184, 86);
            this.connectToControllerButton.TabIndex = 22;
            this.connectToControllerButton.Text = "Connect to Controller";
            this.connectToControllerButton.UseVisualStyleBackColor = false;
            this.connectToControllerButton.Click += new System.EventHandler(this.connectToController);
            // 
            // RAPIDProgramStatusLabel
            // 
            this.RAPIDProgramStatusLabel.AutoSize = true;
            this.RAPIDProgramStatusLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RAPIDProgramStatusLabel.Location = new System.Drawing.Point(936, 152);
            this.RAPIDProgramStatusLabel.Name = "RAPIDProgramStatusLabel";
            this.RAPIDProgramStatusLabel.Size = new System.Drawing.Size(231, 26);
            this.RAPIDProgramStatusLabel.TabIndex = 19;
            this.RAPIDProgramStatusLabel.Text = "RAPID Program Status:";
            this.RAPIDProgramStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // controllerWindowLabel
            // 
            this.controllerWindowLabel.AutoSize = true;
            this.controllerWindowLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.controllerWindowLabel.Location = new System.Drawing.Point(7, 10);
            this.controllerWindowLabel.Name = "controllerWindowLabel";
            this.controllerWindowLabel.Size = new System.Drawing.Size(189, 26);
            this.controllerWindowLabel.TabIndex = 20;
            this.controllerWindowLabel.Text = "Controller Section:";
            this.controllerWindowLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // contrConnectionStatus1
            // 
            this.contrConnectionStatus1.AutoSize = true;
            this.contrConnectionStatus1.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contrConnectionStatus1.ForeColor = System.Drawing.Color.IndianRed;
            this.contrConnectionStatus1.Location = new System.Drawing.Point(1035, 75);
            this.contrConnectionStatus1.Name = "contrConnectionStatus1";
            this.contrConnectionStatus1.Size = new System.Drawing.Size(122, 19);
            this.contrConnectionStatus1.TabIndex = 19;
            this.contrConnectionStatus1.Text = "Not connected";
            // 
            // controllerConnectionStatusLabel
            // 
            this.controllerConnectionStatusLabel.AutoSize = true;
            this.controllerConnectionStatusLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.controllerConnectionStatusLabel.Location = new System.Drawing.Point(936, 39);
            this.controllerConnectionStatusLabel.Name = "controllerConnectionStatusLabel";
            this.controllerConnectionStatusLabel.Size = new System.Drawing.Size(292, 26);
            this.controllerConnectionStatusLabel.TabIndex = 18;
            this.controllerConnectionStatusLabel.Text = "Controller Connection Status:";
            this.controllerConnectionStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1500, 924);
            this.Controls.Add(this.ControllerPanel);
            this.Controls.Add(this.TCPPanel);
            this.Controls.Add(this.DataPanel);
            this.Name = "MainWindow";
            this.Text = "Network Scanning Window";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.DataPanel.ResumeLayout(false);
            this.DataPanel.PerformLayout();
            this.TCPPanel.ResumeLayout(false);
            this.TCPPanel.PerformLayout();
            this.ControllerPanel.ResumeLayout(false);
            this.ControllerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        private void listView2_DoubleClick_1(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Panel DataPanel;
        private System.Windows.Forms.Button startRapidProgButton;
        private System.Windows.Forms.Button disconnectContButton;
        private System.Windows.Forms.Button stopRAPIDProgButton;
        private System.Windows.Forms.Button tcpButton;
        private System.Windows.Forms.Label Progress1;
        private System.Windows.Forms.Panel TCPPanel;
        private System.Windows.Forms.Label clientMessLabel;
        private System.Windows.Forms.Label updateLabel;
        private System.Windows.Forms.Label clientMessSectionLabel;
        private System.Windows.Forms.TextBox PortTextBox;
        private System.Windows.Forms.Label PortLabel;
        private System.Windows.Forms.Label IPAddrLabel;
        private System.Windows.Forms.TextBox IPAddressTextBox;
        private System.Windows.Forms.Label pathViabilityLabel;
        private System.Windows.Forms.Label pathViabilityLabelValue;
        private System.Windows.Forms.Button CPTButton;
        private System.Windows.Forms.Label pathPlanningModeSectionLabel;
        private System.Windows.Forms.Panel ControllerPanel;
        private System.Windows.Forms.Label controllerWindowLabel;
        private System.Windows.Forms.Label contrConnectionStatus1;
        private System.Windows.Forms.Label TCPWindowLabel;
        private System.Windows.Forms.Label RAPIDProgramStatusLabel;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Button connectToControllerButton;
        private System.Windows.Forms.Label servMessSectionLabel;
        private System.Windows.Forms.Label servMessLabel;
        private System.Windows.Forms.Label errMessLabel;
        private System.Windows.Forms.Label errMessSectionLabel;
        private System.Windows.Forms.Label contrConnectionStatusLabel2;
        private System.Windows.Forms.Label contrConnectionStatus2;
        private System.Windows.Forms.Label contrConnectionStatusLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label rapidProgramStatus2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label rapidProgramStatus1;
        private System.Windows.Forms.Label controllerConnectionStatusLabel;
        private System.Windows.Forms.Label rotListViewLabel;
        private System.Windows.Forms.Label posListViewLabel;
        public System.Windows.Forms.ListView rotationListView;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.TextBox zoneBox;
        private System.Windows.Forms.TextBox xBoxPos;
        private System.Windows.Forms.Button savePointsButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox speedBox;
        private System.Windows.Forms.TextBox yBoxPos;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox zBoxPos;
        public System.Windows.Forms.ListView positionListView;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.Label ptpIndexLabel;
        private System.Windows.Forms.Label ptpIndexSectionLabel;
        private System.Windows.Forms.Button goForwardButton;
        private System.Windows.Forms.Button goBackwardButton;
        private System.Windows.Forms.Button CPTBButton;
        private System.Windows.Forms.Button clearDataButton;
        private System.Windows.Forms.Button sendDataButton;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.Label pointCounterLabelValue;
        private System.Windows.Forms.Label pointCounterLabel;
    }
}

