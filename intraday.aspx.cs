﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

namespace Analytics
{
    public partial class intraday : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmailId"] != null)
            {
                if (!IsPostBack)
                {
                    ViewState["FromDate"] = null;
                    ViewState["ToDate"] = null;
                    ViewState["FetchedData"] = null;
                }
                if (Request.QueryString["script"] != null)
                {
                    //labelTitle.Text = Request.QueryString["script"].ToString();
                    //if (Request.QueryString["size"] != null)
                    //    ddlOutputSize.SelectedValue = Request.QueryString["size"];

                    //if(!IsPostBack)

                    ShowGraph(Request.QueryString["script"].ToString());
                    headingtext.InnerText = "Intra-day - " + Request.QueryString["script"].ToString();
                    if (panelWidth.Value != "" && panelHeight.Value != "")
                    {
                        //GetDaily(scriptName);
                        chartintraGraph.Visible = true;
                        chartintraGraph.Width = int.Parse(panelWidth.Value);
                        chartintraGraph.Height = int.Parse(panelHeight.Value);
                    }
                }
                else
                {
                    Response.Redirect(".\\" + Request.QueryString["parent"].ToString());
                }
            }
            else
            {
                Response.Redirect(".\\Default.aspx");
            }
        }
        public void ShowGraph(string scriptName)
        {
            string folderPath = Server.MapPath("~/scriptdata/");
            bool bIsTestOn = true;
            DataTable scriptData = null;
            DataTable tempData = null;
            string expression = "";
            string outputSize = "";
            string interval = "";
            string fromDate = "", toDate = "";
            DataRow[] filteredRows = null;


            if (ViewState["FetchedData"] == null)
            {
                if (Session["IsTestOn"] != null)
                {
                    bIsTestOn = System.Convert.ToBoolean(Session["IsTestOn"]);
                }

                if (Session["TestDataFolder"] != null)
                {
                    folderPath = Session["TestDataFolder"].ToString();
                }
                if ( (Request.QueryString["size"] != null) && (Request.QueryString["interval"] != null))
                {
                    outputSize = Request.QueryString["size"].ToString();
                    interval = Request.QueryString["interval"];
                    scriptData = StockApi.getIntraday(folderPath, scriptName, time_interval:interval, outputsize: outputSize, bIsTestModeOn: bIsTestOn, bSaveData: false);
                }
                ViewState["FetchedData"] = scriptData;
            }
            else
            {
                if (ViewState["FromDate"] != null)
                    fromDate = ViewState["FromDate"].ToString();
                if (ViewState["ToDate"] != null)
                    toDate = ViewState["ToDate"].ToString();

                if ((fromDate.Length > 0) && (toDate.Length > 0))
                {
                    tempData = (DataTable)ViewState["FetchedData"];
                    expression = "Date >= '" + fromDate + "' and Date <= '" + toDate + "'";
                    filteredRows = tempData.Select(expression);
                    if( (filteredRows != null) && (filteredRows.Length >0))
                        scriptData = filteredRows.CopyToDataTable();
                }
                else
                {
                    scriptData = (DataTable)ViewState["FetchedData"];
                }
            }
            if (scriptData != null)
            {
                chartintraGraph.DataSource = scriptData;
                chartintraGraph.DataBind();
                chartintraGraph.ChartAreas[0].AxisX.LabelStyle.Format = "g";
                if (checkBoxOpen.Checked)
                    showOpenLine(scriptData);
                if (checkBoxHigh.Checked)
                    showHighLine(scriptData);
                if (checkBoxLow.Checked)
                    showLowLine(scriptData);
                if (checkBoxClose.Checked)
                    showCloseLine(scriptData);
                if (checkBoxCandle.Checked)
                    showCandleStickGraph(scriptData);
                if (checkBoxVolume.Checked)
                    showVolumeGraph(scriptData);
                if (checkBoxGrid.Checked)
                {
                    GridViewDaily.Visible = true;
                    GridViewDaily.DataSource = scriptData;
                    GridViewDaily.DataBind();
                }
            }
        }

        public void showCloseLine(DataTable scriptData)
        {
            (chartintraGraph.Series["Close"]).XValueMember = "Date";
            (chartintraGraph.Series["Close"]).XValueType = ChartValueType.DateTime;
            (chartintraGraph.Series["Close"]).YValueMembers = "Close";
            //(chartdailyGraph.Series["Close"]).ToolTip = "Close:   Date:#VALX;   Value:#VALY";
        }

        public void showOpenLine(DataTable scriptData)
        {
            (chartintraGraph.Series["Open"]).XValueMember = "Date";
            (chartintraGraph.Series["Open"]).XValueType = ChartValueType.DateTime;
            (chartintraGraph.Series["Open"]).YValueMembers = "Open";

            //(chartdailyGraph.Series["Close"]).ToolTip = "Close:   Date:#VALX;   Value:#VALY";
        }
        public void showHighLine(DataTable scriptData)
        {
            (chartintraGraph.Series["High"]).XValueMember = "Date";
            (chartintraGraph.Series["High"]).XValueType = ChartValueType.DateTime;
            (chartintraGraph.Series["High"]).YValueMembers = "High";
            //(chartdailyGraph.Series["Close"]).ToolTip = "Close:   Date:#VALX;   Value:#VALY";
        }
        public void showLowLine(DataTable scriptData)
        {
            (chartintraGraph.Series["Low"]).XValueMember = "Date";
            (chartintraGraph.Series["Low"]).XValueType = ChartValueType.DateTime;
            (chartintraGraph.Series["Low"]).YValueMembers = "Low";
            //(chartdailyGraph.Series["Close"]).ToolTip = "Close:   Date:#VALX;   Value:#VALY";
        }

        public void showCandleStickGraph(DataTable scriptData)
        {
            //chartdailyGraph.DataSource = scriptData;
            chartintraGraph.Series["OHLC"].XValueMember = "Date";
            chartintraGraph.Series["OHLC"].YValueMembers = "Open, High, Low, Close";
            //chartdailyGraph.DataBind();

            chartintraGraph.Series["OHLC"].BorderColor = System.Drawing.Color.Black;
            chartintraGraph.Series["OHLC"].Color = System.Drawing.Color.Black;
            chartintraGraph.Series["OHLC"].CustomProperties = "PriceDownColor=Blue, PriceUpColor=Red";
            chartintraGraph.Series["OHLC"].XValueType = ChartValueType.DateTime;
            chartintraGraph.Series["OHLC"]["OpenCloseStyle"] = "Triangle";
            chartintraGraph.Series["OHLC"]["ShowOpenClose"] = "Both";
            //chartdailyGraph.Series["OHLC"]["PriceDownColor"] = "Triangle";
            //chartdailyGraph.Series["OHLC"]["PriceUpColor"] = "Both";

            chartintraGraph.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
            chartintraGraph.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;
            chartintraGraph.ChartAreas[0].AxisY.Minimum = 0;
            //chartintraGraph.ChartAreas[0].AxisY.Maximum = chartdailyGraph.Series["OHLC"].Points.FindMaxByValue("Y1", 0).YValues[0];
            chartintraGraph.DataManipulator.IsStartFromFirst = true;
        }
        public void showVolumeGraph(DataTable scriptData)
        {
            (chartintraGraph.Series["Volume"]).XValueMember = "Date";
            (chartintraGraph.Series["Volume"]).XValueType = ChartValueType.DateTime;
            (chartintraGraph.Series["Volume"]).YValueMembers = "Volume";
            //(chartdailyGraph.Series["Volume"]).ToolTip = "Date:#VALX;   Volume:#VALY";
        }

        protected void buttonShowGraph_Click(object sender, EventArgs e)
        {
            string fromDate = textboxFromDate.Text;
            string toDate = textboxToDate.Text;
            string scriptName = Request.QueryString["script"].ToString();
            ViewState["FromDate"] = textboxFromDate.Text;
            ViewState["ToDate"] = textboxToDate.Text;
            ShowGraph(scriptName);
        }

        protected void checkBoxOpen_CheckedChanged(object sender, EventArgs e)
        {
            string scriptName = Request.QueryString["script"].ToString();
            ShowGraph(scriptName);
        }

        protected void checkBoxHigh_CheckedChanged(object sender, EventArgs e)
        {
            string scriptName = Request.QueryString["script"].ToString();
            ShowGraph(scriptName);
        }

        protected void checkBoxLow_CheckedChanged(object sender, EventArgs e)
        {
            string scriptName = Request.QueryString["script"].ToString();
            ShowGraph(scriptName);
        }

        protected void checkBoxClose_CheckedChanged(object sender, EventArgs e)
        {
            string scriptName = Request.QueryString["script"].ToString();
            ShowGraph(scriptName);
        }

        protected void checkBoxCandle_CheckedChanged(object sender, EventArgs e)
        {
            string scriptName = Request.QueryString["script"].ToString();
            ShowGraph(scriptName);
        }

        protected void checkBoxGrid_CheckedChanged(object sender, EventArgs e)
        {
            string scriptName = Request.QueryString["script"].ToString();
            ShowGraph(scriptName);
        }

        protected void checkBoxVolume_CheckedChanged(object sender, EventArgs e)
        {
            string scriptName = Request.QueryString["script"].ToString();
            ShowGraph(scriptName);
        }
    }
}