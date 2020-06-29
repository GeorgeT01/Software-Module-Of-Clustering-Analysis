using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ConsoleTables;
using System.IO;

namespace Cluster
{


    public partial class Form1 : Form
    {
    /// <summary>
    /// K-Means
    /// </summary>
        List<double> list1;
        List<double> list2;

        List<double> clusterlist1 = new List<double>();
        List<double> clusterlist2 = new List<double>();

        double[] P1 = new double[2];
        double[] P2 = new double[2];
        double[] P3 = new double[2];
        bool started = false;
        bool converted = false;
        bool isResult = false;

        List<double> clusterObjects11 = new List<double>();
        List<double> clusterObjects12 = new List<double>();
        List<double> clusterObjects21 = new List<double>();
        List<double> clusterObjects22 = new List<double>();
        List<double> clusterObjects31 = new List<double>();
        List<double> clusterObjects32 = new List<double>();



        /// <summary>
        /// for Maximin
        /// </summary>
        List<double> listMaximin1;
        List<double> listMaximin2;
        bool isConverted = false;
        bool firstCalculate = false;
        bool secondCalculate = false;
        bool isResultMaximin = false;
        double[] pMaximin = new double[2];
        int prototypeCounter = 1;
        List<int> prototypesList = new List<int>();
        List<double> distancesListMaximin = new List<double>();
        int maxIndexMaximin;
        double thresholdDistance;



        public Form1()
        {
            InitializeComponent();
        }

        private void performBtn_Click(object sender, EventArgs e)
        {
            //check if file is open
            if (!textInput1.Text.Equals(""))
            {
                // choose method
                if (algorithmcomboBox.SelectedIndex == 0)
                {
                    kmeans(list1, list2);
                }
                else if (algorithmcomboBox.SelectedIndex == 1)
                {
                    maximin();
                }
                else {
                    MessageBox.Show("Please, Choose Method");
                }
            }
            else {
                MessageBox.Show("Please, Choose file first");
            }
        }

        public void maximin() {

            if (listMaximin1.Count == listMaximin2.Count)
            {
                if (!isConverted)
                {
                    convertMaximin();
                }
                else
                {
                    if (!firstCalculate)
                    {
                        firstCalcMaximin();
                    }
                    else
                    {
                        if (!secondCalculate)
                        {
                            secondCalcMaximin();
                        }
                        else
                        {
                            if (!isResultMaximin)
                            {
                                compareMaximin();
                            }

                        }


                    }

                }


            }
            else {
                MessageBox.Show("Length dose not match!");
            }
        }


        /// <summary>
        /// Maximin Algorithm
        /// </summary>
        /// 
        public void convertMaximin()
        {
            consoleTextBox.AppendText("<<Maximin Method>>\n");
            isConverted = true;
            performBtn.Text = "Next >";
            listMaximin1 = convertIntoDimensionlessNumbers(listMaximin1);
            listMaximin2 = convertIntoDimensionlessNumbers(listMaximin2);

            // add to table
            for (int i = 1; i <= listMaximin1.Count; i++)
            {
                // add columns
                dataGridView1.Columns.Add("C", "C" + i);
            }
            // add multiple rows
            for (int i = 0; i < 2; i++)
            {
                dataGridView1.Rows.Add();
            }
            for (int i = 0; i < listMaximin1.Count; i++)
            {
                dataGridView1.Rows[0].Cells[i].Value = listMaximin1[i];
                dataGridView1.Rows[1].Cells[i].Value = listMaximin2[i];
            }
            consoleTextBox.AppendText("Convert into dimensionless numbers:\n");
            foreach (var obj1 in listMaximin1)
            {
                consoleTextBox.AppendText(obj1.ToString() + ", ");
            }
            consoleTextBox.AppendText("\n");

            foreach (var obj2 in listMaximin2)
            {
                consoleTextBox.AppendText(obj2.ToString() + ", ");
            }
            consoleTextBox.AppendText("\n");

        }

       

        public void firstCalcMaximin() {
            firstCalculate = true;
            dataGridView1.Rows.Clear();
            for (int i = 0; i < 2; i++)
            {
                dataGridView1.Rows.Add();
            }
            pMaximin[0] = listMaximin1[0];
            pMaximin[1] = listMaximin2[0];

            prototypesList.Add(0);
            dataGridView1.Rows[prototypeCounter - 1].HeaderCell.Value = "P" + prototypeCounter + " = C1";
            consoleTextBox.AppendText("the first object, C1, as the prototype of the first class:\n");
            consoleTextBox.AppendText("P1 = C1 = ("+ pMaximin[0] + ";"+ pMaximin[1] + ")\n");

            consoleTextBox.AppendText("Calculate the distances from the prototype P1 to all other object\n");
            for (int i = 0; i < listMaximin1.Count; i++)
            {
                double D;
                if (i == 0)
                {
                    D = 0;
                }
                else
                {
                    D = distanceCalculation(listMaximin1[i], pMaximin[0], listMaximin2[i], pMaximin[1]);
                }
                dataGridView1.Rows[0].Cells[i].Value = D; //add to table
                distancesListMaximin.Add(D);// add to list
                consoleTextBox.AppendText("D(P1, C"+(i+1) +") = "+ D +"\n");

            }

        }

        public void secondCalcMaximin()
        {
            secondCalculate = true;
            maxIndexMaximin = distancesListMaximin.IndexOf(distancesListMaximin.Max(t => t));//get index of largest
            consoleTextBox.AppendText("The object farthest from the prototype P1 is C"+(maxIndexMaximin+1)+"\n");
            consoleTextBox.AppendText("P2 = C"+(maxIndexMaximin+1)+" = ("+ listMaximin1[maxIndexMaximin]+";"+ listMaximin2[maxIndexMaximin] + ")\n");
            consoleTextBox.AppendText("Calculate the threshold distance:\n");
            thresholdDistance = distanceCalculation(pMaximin[0], listMaximin1[maxIndexMaximin], pMaximin[1], listMaximin2[maxIndexMaximin]) / 2;
            consoleTextBox.AppendText("T = " + thresholdDistance + "\n");
            pMaximin[0] = listMaximin1[maxIndexMaximin];
            pMaximin[1] = listMaximin2[maxIndexMaximin];
            prototypesList.Add(maxIndexMaximin);

            dataGridView1.Rows.Add();
            prototypeCounter++;
            // 4.	We calculate the threshold distance
            consoleTextBox.AppendText("Calculate the distances from each prototype to each object:\n");
            int newINdexToShow = maxIndexMaximin + 1;
            dataGridView1.Rows[prototypeCounter - 1].HeaderCell.Value = "P" + prototypeCounter + " = C" + newINdexToShow;
            //5.We use the formula (1) to calculate the distances from each prototype to each object
            distancesListMaximin.Clear();
            for (int i = 0; i < listMaximin1.Count; i++)
            {
                double D;
                if (i == maxIndexMaximin)
                {
                    D = 0;
                }
                else
                {
                    D = distanceCalculation(listMaximin1[i], pMaximin[0], listMaximin2[i], pMaximin[1]);
                }
                dataGridView1.Rows[prototypeCounter - 1].Cells[i].Value = D; //add to table
                distancesListMaximin.Add(D);// add to list
                consoleTextBox.AppendText("D(P2, C" + (i + 1) + ") = " + D + "\n");
            }
            //cluster calc
            //first find max
            double max = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (max < Convert.ToDouble(cell.Value))
                    {
                        max = Convert.ToDouble(cell.Value);
                    }
                }
            }
            dataGridView1.Rows[prototypeCounter].HeaderCell.Value = "Cluster";
            List<double> listmin = new List<double>();
            double min;
            int minIndex;
            double maxInRow = 0;
            int maxInRowIndex = 0;
            // calc cluster
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                if (maxInRow < Convert.ToDouble(dataGridView1.Rows[prototypeCounter - 1].Cells[i].Value))
                {
                    maxInRow = Convert.ToDouble(dataGridView1.Rows[prototypeCounter - 1].Cells[i].Value);
                    maxInRowIndex = dataGridView1.Rows[prototypeCounter - 1].Cells[i].RowIndex;
                }
                min = Convert.ToDouble(dataGridView1.Rows[0].Cells[i].Value);
                minIndex = dataGridView1.Rows[0].Cells[i].RowIndex;
                for (int j = 0; j < dataGridView1.Rows.Count; j++)
                {
                    // check if cell not empty
                    if (dataGridView1.Rows[j].Cells[i].Value == null || dataGridView1.Rows[j].Cells[i].Value == DBNull.Value || String.IsNullOrWhiteSpace(dataGridView1.Rows[j].Cells[i].Value.ToString()))
                    {
                    }
                    else
                    {

                        if (min > Convert.ToDouble(dataGridView1.Rows[j].Cells[i].Value))
                        {
                            min = Convert.ToDouble(dataGridView1.Rows[j].Cells[i].Value);
                            listmin.Add(Convert.ToDouble(dataGridView1.Rows[j].Cells[i].Value));
                            minIndex = dataGridView1.Rows[j].Cells[i].RowIndex;
                        }

                    }
                }
                dataGridView1.Rows[prototypeCounter].Cells[i].Value = (minIndex + 1);
                // Console.WriteLine("MIN=" + minIndex + 1);
                // Console.WriteLine("----INdex=" + minIndex+ 1);
            }
        }
        public void compareMaximin()
        {
            List<clusterListValues> clusterListValues = new List<clusterListValues>();

            for (int j = 0; j < dataGridView1.Columns.Count; j++)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {

                    int index = dataGridView1.Rows[prototypeCounter].Cells[j].ColumnIndex;

                    if (Convert.ToInt64(dataGridView1.Rows[prototypeCounter].Cells[j].Value) == i + 1)
                    {

                        clusterListValues.Add(new Cluster.clusterListValues(Convert.ToDouble(dataGridView1.Rows[i].Cells[index].Value), i + 1));
                    }

                }

            }

            List<int> listOfClusters = new List<int>();

            for (int z = 0; z < dataGridView1.Rows[prototypeCounter].Cells.Count; z++)
            {
                listOfClusters.Add(Convert.ToInt16(dataGridView1.Rows[prototypeCounter].Cells[z].Value));
            }
            List<int> noDupes = listOfClusters.Distinct().ToList();

            //foreach (int obj in noDupes) {
            //    Console.WriteLine("no dupes = {0}", obj);
            //}
            var newList = clusterListValues.OrderBy(x => x.ClusterNumber).ToList();

            //foreach (var item in newList) {
            //    Console.WriteLine(item.ClusterNumber+" = "+item.ValueNumber);
            //}
       
            if (!isResultMaximin)
            {
                // find farthest object
                // consoleTextBox.AppendText("farthest object = " + maxValueInList.ToString() + "\n");
            
            foreach (int obj in noDupes)
            {
                var values = clusterListValues.Where(t => t.ClusterNumber == obj).ToList();
                double maxValueInList = values.Max(t => t.ValueNumber);

                if (!isResultMaximin)
                {
                    // find farthest object
                    // consoleTextBox.AppendText("farthest object = " + maxValueInList.ToString() + "\n");
                }
                for (int i = 0; i < values.Count; i++)
                { // loop through list
                    if (!isResultMaximin)
                    {

                        if (values[i].ValueNumber == maxValueInList)
                        { // find index 
                            //Console.WriteLine("index of {0} = {1}", maxValueInList, clusterListValues.FindIndex(a => a.ValueNumber == maxValueInList));
                            consoleTextBox.AppendText("The object farthest from P is C" + (clusterListValues.FindIndex(a => a.ValueNumber == maxValueInList) + 1) + "\n");
                            calcNewPrototype(maxValueInList, clusterListValues.FindIndex(a => a.ValueNumber == maxValueInList));


                        }
                    }
                }
            }
            // We calculate new threshold distance
            //thresholdDistance = distanceCalculation(pMaximin[0], listMaximin1[maxIndexMaximin], pMaximin[1], listMaximin2[maxIndexMaximin]) / 2;
            //Console.WriteLine("Threshold Distance = {0}", thresholdDistance);
            //foreach (var obj in prototypesList) {
            //    Console.WriteLine(obj);
            //}
       
                // consoleTextBox.AppendText("Calculate new threshold distance\n");
                //prototypesList.Sort();

                double distancesSum = 0;
                int distancesNumber = 0;
                //consoleTextBox.AppendText("prototypesList:\n");
                for (int i = 0; i < prototypesList.Count; i++)
                {
                    //consoleTextBox.AppendText(prototypesList[i] + "\n");
                    for (int j = 0; j < prototypesList.Count; j++)
                    {
                        if (j > i)
                        {
                            distancesNumber++;
                            distancesSum += distanceCalculation(listMaximin1[prototypesList[i]], listMaximin1[prototypesList[j]], listMaximin2[prototypesList[i]], listMaximin2[prototypesList[j]]);
                            //Console.WriteLine("D({0},{1},{2},{3})", listMaximin1[prototypesList[i]], listMaximin1[prototypesList[j]], listMaximin2[prototypesList[i]], listMaximin2[prototypesList[j]]);
                            //Console.WriteLine(distanceCalculation(listMaximin1[prototypesList[i]], listMaximin1[prototypesList[j]], listMaximin2[prototypesList[i]], listMaximin2[prototypesList[j]]));
                            //consoleTextBox.AppendText("D(" + listMaximin1[prototypesList[i]] + "," + listMaximin1[prototypesList[j]] + "," + listMaximin2[prototypesList[i]] + "," + listMaximin2[prototypesList[j]] + ")\n");
                            //consoleTextBox.AppendText(distanceCalculation(listMaximin1[prototypesList[i]], listMaximin1[prototypesList[j]], listMaximin2[prototypesList[i]], listMaximin2[prototypesList[j]]) + "\n");
                        }
                    }
                }
                thresholdDistance = distancesSum / (distancesNumber * 2);

                consoleTextBox.AppendText("The threshold distance\n");
                consoleTextBox.AppendText("T = " + distancesSum +" / "+ distancesNumber+" x "+ 2 +"" + "\n");
                consoleTextBox.AppendText("T = " + thresholdDistance + "\n");
            }
            foreach (int obj in noDupes)
            {
                var values = clusterListValues.Where(t => t.ClusterNumber == obj).ToList();
                double maxValueInList = values.Max(t => t.ValueNumber);


                for (int i = 0; i < values.Count; i++)
                { // loop through list
                    if (!isResultMaximin)
                    {

                        if (values[i].ValueNumber == maxValueInList)
                        { // find index 
                            //Console.WriteLine("index of {0} = {1}", maxValueInList, clusterListValues.FindIndex(a => a.ValueNumber == maxValueInList));
                            compareFinal(maxValueInList);
                        }
                    }
                }
            }
        }
        public bool isMsgBox = false;

        public void compareFinal(double value) {
            if (thresholdDistance < value)
            {
                if (!isMsgBox)
                {
                    //dataGridView1.Rows.RemoveAt(prototypeCounter - 1);
                    MessageBox.Show("The current subdivision is final");
                    consoleTextBox.AppendText("All objects(even the farthest object) are sufficiently close to their prototypes\n");
                    consoleTextBox.AppendText("The current subdivision is final!\n");
                    isMsgBox = true;
                }
                isResultMaximin = true;
                performBtn.Text = "Done";

            }

        }
        public void calcNewPrototype(double value, int index)
        {

            consoleTextBox.AppendText("Compare the distance from the prototype to the farthest object\n");

            dataGridView1.Rows.Add();
          
                double dis = distanceCalculation(listMaximin1[index], pMaximin[0], listMaximin2[index], pMaximin[1]);
                pMaximin[0] = listMaximin1[index];
                pMaximin[1] = listMaximin2[index];
                prototypesList.Add(index);
                //consoleTextBox.AppendText(" prototypesList.Add("+index+");\n");

                prototypeCounter++;

                consoleTextBox.AppendText("P" + prototypeCounter + "(" + pMaximin[0] + ";" + pMaximin[1] + ")\n");
                //Console.WriteLine("P({0};{1})", pMaximin[0], pMaximin[1]);
                int newINdex = index + 1;
                // 4.	We calculate the distance 
                consoleTextBox.AppendText("Calculate the distance\n");
                dataGridView1.Rows[prototypeCounter - 1].HeaderCell.Value = "P" + prototypeCounter + " = C" + newINdex;
                for (int i = 0; i < listMaximin1.Count; i++)
                {
                    double D;
                    if (i == index)
                    {
                        D = 0;
                    }
                    else
                    {
                        D = distanceCalculation(listMaximin1[i], pMaximin[0], listMaximin2[i], pMaximin[1]);
                    }
                    dataGridView1.Rows[prototypeCounter - 1].Cells[i].Value = D; //add to table
                    //distancesListMaximin.Add(D);// add to list
                    //Console.WriteLine("D(C{0},P1) = {1}", i + 1, D);
                    consoleTextBox.AppendText("D(C" + (i + 1) + ",P" + prototypeCounter + ") = "+D+"\n");
                }
                dataGridView1.Rows[prototypeCounter].HeaderCell.Value = "Cluster";
                List<double> listmin = new List<double>();
                double min;
                int minIndex;
                double maxInRow = 0;
                int maxInRowIndex = 0;
                // calc cluster
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    if (maxInRow < Convert.ToDouble(dataGridView1.Rows[prototypeCounter - 1].Cells[i].Value))
                    {
                        maxInRow = Convert.ToDouble(dataGridView1.Rows[prototypeCounter - 1].Cells[i].Value);
                        maxInRowIndex = dataGridView1.Rows[prototypeCounter - 1].Cells[i].RowIndex;
                    }
                    min = Convert.ToDouble(dataGridView1.Rows[0].Cells[i].Value);
                    minIndex = dataGridView1.Rows[0].Cells[i].RowIndex;
                    for (int j = 0; j < dataGridView1.Rows.Count; j++)
                    {
                        // check if cell not empty
                        if (dataGridView1.Rows[j].Cells[i].Value == null || dataGridView1.Rows[j].Cells[i].Value == DBNull.Value || String.IsNullOrWhiteSpace(dataGridView1.Rows[j].Cells[i].Value.ToString()))
                        {
                        }
                        else
                        {

                            if (min > Convert.ToDouble(dataGridView1.Rows[j].Cells[i].Value))
                            {
                                min = Convert.ToDouble(dataGridView1.Rows[j].Cells[i].Value);
                                listmin.Add(Convert.ToDouble(dataGridView1.Rows[j].Cells[i].Value));
                                minIndex = dataGridView1.Rows[j].Cells[i].RowIndex;
                            }

                        }
                    }
                    dataGridView1.Rows[prototypeCounter].Cells[i].Value = minIndex + 1;
                   // Console.WriteLine("MIN=" + min);
                }

        


        }


        /// <summary>
        ///  K-means algorithm
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        public void kmeans(List<double> list1, List<double> list2)
        {

            try {
                // check if objects are equal
                if (list1.Count == list2.Count)
                {
                    int length = list1.Count; // to use in(for loops)

                    if (!converted)
                    {
                        ConvertCalc();
                    }
                    else {
                        if (!started)
                        {
                            firstCalc();
                        }
                        else
                        {
                            if (!isResult) {
                                newCalc();
                            }
                        }
                    }
                  


                }
                else {
                    MessageBox.Show("Number of objects does not match, please check .txt file and try again");
                }

            } catch (Exception ex) {
                MessageBox.Show("Error: Could not preform. Original error: " + ex.Message);
            }


        }

        public List<double> convertIntoDimensionlessNumbers(List<double> list) {
            List<double> result = new List<double>();
            var max = list.Max();
            foreach (double obj in list) {
                result.Add(obj / max);
            }
            return result;
        }

        public double distanceCalculation(double val1, double val2, double val3, double val4) {

            double res = Math.Sqrt(Math.Pow(val1 - val2, 2) + Math.Pow(val3 - val4, 2));

            return res;
        }

        public void ConvertCalc() {
            performBtn.Text = "Next >";
            consoleTextBox.AppendText("<<K-Means Method>>\n");
            converted = true;
            consoleTextBox.AppendText("Convert into dimensionless numbers:\n");

            list1 = convertIntoDimensionlessNumbers(list1);
            list2 = convertIntoDimensionlessNumbers(list2);
            // add to table
            for (int i = 1; i <= list1.Count; i++)
            {
                // add columns
                dataGridView1.Columns.Add("C", "C" + i);
            }
            // add two rows
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();

            for (int i = 0; i < list1.Count; i++)
            {
                dataGridView1.Rows[0].Cells[i].Value = list1[i];
                dataGridView1.Rows[1].Cells[i].Value = list2[i];
            }
            foreach (var obj1 in list1) {
                consoleTextBox.AppendText(obj1.ToString()+", ");
            }
            consoleTextBox.AppendText("\n");
            foreach (var obj2 in list2)
            {
                consoleTextBox.AppendText(obj2.ToString() + ", ");
            }
            consoleTextBox.AppendText("\n");
        }

        public void firstCalc()
        {

            started = true;
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            List<double> groupList = list2;
            if (groupsComboBox.SelectedIndex == 0) {
               groupList = list1;
            }
            // (1) choose the most appropriate object (the prototype)
            double min = groupList.Min(t => t);
            double max = groupList.Max(t => t);
            //get avarage of list
            double averageValueFrom = groupList.Average();
            // get nearst value to average  from list
            var average = groupList.OrderBy(x => Math.Abs((double)x - averageValueFrom)).First();
            // get last item from list if exist
            int averageIndex = 0;
            for (int i = 0; i < groupList.Count; i++)
            {
                if (groupList[i] == average)
                {
                    averageIndex = i;
                }
            }
            //get index of the objects
            var minIndex = groupList.IndexOf(groupList.Min(t => t));
            var maxIndex = groupList.IndexOf(groupList.Max(t => t));
            if (groupsComboBox.SelectedIndex == 0)
            {

                switch (rateComboBox.SelectedIndex) {
                    case 0:
                        P1[0] = min; P1[1] = list2[minIndex]; // low
                        P2[0] = max; P2[1] = list2[maxIndex]; // high
                        P3[0] = average; P3[1] = list2[averageIndex]; // average
                    break;
                    case 1:
                        P1[0] = max; P1[1] = list2[maxIndex]; // high
                        P2[0] = min; P2[1] = list2[minIndex]; // low
                        P3[0] = average; P3[1] = list2[averageIndex]; // average
                        break;
                    case 2:
                        P1[0] = average; P1[1] = list2[averageIndex]; // average 
                        P2[0] = min; P2[1] = list2[minIndex]; // low
                        P3[0] = max; P3[1] = list2[maxIndex]; // high
                        break;
                    case 3:
                        P1[0] = min;P1[1] = list2[minIndex]; // low
                        P2[0] = average;P2[1] = list2[averageIndex]; // average
                        P3[0] = max;P3[1] = list2[maxIndex]; // high
                        break;
                    case 4:
                        P1[0] = max; P1[1] = list2[maxIndex]; // high
                        P2[0] = average; P2[1] = list2[averageIndex]; // average
                        P3[0] = min; P3[1] = list2[minIndex]; // low
                        break;
                    case 5:
                        P1[0] = average; P1[1] = list2[averageIndex]; // average
                        P2[0] = max; P2[1] = list2[maxIndex]; // high
                        P3[0] = min; P3[1] = list2[minIndex]; // 
                        break;
                    case 6:
                        P1[0] = max; P1[1] = list2[maxIndex]; // high
                        P2[0] = max; P2[1] = list2[maxIndex]; // high
                        P3[0] = min; P3[1] = list2[minIndex]; // low
                        break;

                }

            }
            else {

             


                switch (rateComboBox.SelectedIndex)
                {
                    case 0:
                        P1[0] = list1[minIndex]; P1[1] = min; // low
                        P2[0] = list1[maxIndex]; P2[1] = max; // high
                        P3[0] = list1[averageIndex]; P3[1] = average; // average
                        break;
                    case 1:
                        P1[0] = list1[maxIndex]; P1[1] = max; // high
                        P2[0] = list1[minIndex]; P2[1] = min; // low
                        P3[0] = list1[averageIndex]; P3[1] = average; // average
                        break;
                    case 2:
                        P1[0] = list1[averageIndex]; P1[1] = average; // average
                        P2[0] = list1[minIndex]; P2[1] = min; // low
                        P3[0] = list1[maxIndex]; P3[1] = max; // high
                        break;
                    case 3:
                        P1[0] = list1[minIndex]; P1[1] = min; // low
                        P2[0] = list1[averageIndex]; P2[1] = average; // average
                        P3[0] = list1[maxIndex]; P3[1] = max; // hight
                        break;
                    case 4:
                        P1[0] = list1[maxIndex]; P1[1] = max; // max
                        P2[0] = list1[averageIndex]; P2[1] = average; // average
                        P3[0] = list1[minIndex]; P3[1] = min; // low
                        break;
                    case 5:
                        P1[0] = list1[averageIndex]; P1[1] = average; // average
                        P2[0] = list1[maxIndex]; P2[1] = max; // high
                        P3[0] = list1[minIndex]; P3[1] = min; // low
                        break;
                    case 6:
                        P1[0] = list1[maxIndex]; P1[1] = max; // high
                        P2[0] = list1[maxIndex]; P2[1] = max; // high
                        P3[0] = list1[minIndex]; P3[1] = min; // low
                        break;

                }

            }
            

        

            consoleTextBox.AppendText("\n");
            consoleTextBox.AppendText("Choose the most appropriate object (the prototype):\n");
            consoleTextBox.AppendText("P1 = ("+ P1[0] + ";"+ P1[1] + ")\n");
            consoleTextBox.AppendText("P2 = ("+ P2[0] + ";"+ P2[1] + ")\n");
            consoleTextBox.AppendText("P3 = ("+ P3[0] + ";"+ P3[1] + ")\n");
            consoleTextBox.AppendText("\n");

            
            for (int i = 0; i < 5; i++)
            {
                dataGridView1.Rows.Add();
            }
            dataGridView1.Rows[0].HeaderCell.Value = "P1";
            dataGridView1.Rows[1].HeaderCell.Value = "P2";
            dataGridView1.Rows[2].HeaderCell.Value = "P3";
            dataGridView1.Rows[3].HeaderCell.Value = "Cluster";


            ////////
            ///
            consoleTextBox.AppendText("\n");
            consoleTextBox.AppendText("Calculate the distances from each prototype to each object:\n");
            

            for (int i = 0; i < list1.Count; i++)
            {
                double D1 = distanceCalculation(list1[i], P1[0], list2[i], P1[1]);
                double D2 = distanceCalculation(list1[i], P2[0], list2[i], P2[1]);
                double D3 = distanceCalculation(list1[i], P3[0], list2[i], P3[1]);
                //consoleTextBox.AppendText("calc distince " + list1[i]+", "+ P1[0] + ", " + list2[i] + ", " + P1[1] + ", " + "\n");

                dataGridView1.Rows[0].Cells[i].Value = D1;
                dataGridView1.Rows[1].Cells[i].Value = D2;
                dataGridView1.Rows[2].Cells[i].Value = D3;

                consoleTextBox.AppendText("D(C"+ (i + 1 )+ ",P1) = "+ D1 + "\n");
                consoleTextBox.AppendText("D(C"+ (i + 1 )+ ",P3) = "+ D2 + "\n");
                consoleTextBox.AppendText("D(C"+ (i + 1 )+ ",P3) = "+ D3 + "\n");

                if (D1 < D2 && D1 < D3)
                {
                    consoleTextBox.AppendText("Cluster = 1\n");
                    clusterObjects11.Add(list1[i]);
                    clusterObjects12.Add(list2[i]);
                    dataGridView1.Rows[3].Cells[i].Value = 1;
                    clusterlist1.Add(1);

                }
                else if (D2 < D1 && D2 < D3)
                {
                    consoleTextBox.AppendText("Cluster = 2\n");
                    clusterObjects21.Add(list1[i]);
                    clusterObjects22.Add(list2[i]);
                    dataGridView1.Rows[3].Cells[i].Value = 2;
                    clusterlist1.Add(2);

                }
                else if (D3 < D1 && D3 < D2)
                {
                    consoleTextBox.AppendText("Cluster = 3\n");
                    clusterObjects31.Add(list1[i]);
                    clusterObjects32.Add(list2[i]);
                    dataGridView1.Rows[3].Cells[i].Value = 3;
                    clusterlist1.Add(3);


                }

            }
        }
        public void newCalc() {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            consoleTextBox.AppendText("\n");
            consoleTextBox.AppendText("Calculate the new prototype:\n");
            consoleTextBox.AppendText("cluster objects:\n");
            foreach (var ob in clusterObjects11) {
                consoleTextBox.AppendText(ob.ToString()+"\n");
            }
            foreach (var ob in clusterObjects12)
            {
                consoleTextBox.AppendText(ob.ToString()+"\n");
            }
            double sumCluster1x = 0;
            double sumCluster1y = 0;
            for (int i = 0; i < clusterObjects11.Count; i++)
            {
                sumCluster1x += clusterObjects11[i];
                sumCluster1y += clusterObjects12[i];
            }
            //P1 = (d1x;d1y)
            double d1x = sumCluster1x / clusterObjects11.Count;
            double d1y = sumCluster1y / clusterObjects12.Count;
            consoleTextBox.AppendText("P1 = ("+ d1x + ";"+ d1y + ")\n");
            // 2nd cluster
            double sumCluster2x = 0;
            double sumCluster2y = 0;
            for (int i = 0; i < clusterObjects21.Count; i++)
            {
                sumCluster2x += clusterObjects21[i];
                sumCluster2y += clusterObjects22[i];
            }
            double d2x = sumCluster2x / clusterObjects21.Count;
            double d2y = sumCluster2y / clusterObjects22.Count;
            consoleTextBox.AppendText("P2 = (" + d2x + ";" + d2y + ")\n");

            // 3rd cluster
            double sumCluster3x = 0;
            double sumCluster3y = 0;
            for (int i = 0; i < clusterObjects31.Count; i++)
            {
                sumCluster3x += clusterObjects31[i];
                sumCluster3y += clusterObjects32[i];
            }
            double d3x = sumCluster3x / clusterObjects31.Count;
            double d3y = sumCluster3y / clusterObjects32.Count;
            consoleTextBox.AppendText("P3 = (" + d3x + ";" + d3y + ")\n");


            for (int i = 0; i < 5; i++)
            {
                dataGridView1.Rows.Add();
            }
            dataGridView1.Rows[0].HeaderCell.Value = "P1";
            dataGridView1.Rows[1].HeaderCell.Value = "P2";
            dataGridView1.Rows[2].HeaderCell.Value = "P3";
            dataGridView1.Rows[3].HeaderCell.Value = "Cluster";

            consoleTextBox.AppendText("\n");
            consoleTextBox.AppendText("Calculate the distances from each prototype to each object:\n");

            clusterObjects11.Clear();
            clusterObjects12.Clear();
            clusterObjects21.Clear();
            clusterObjects22.Clear();
            clusterObjects31.Clear();
            clusterObjects32.Clear();
            for (int i = 0; i < list1.Count; i++)
            {
                double D1 = distanceCalculation(list1[i], d1x, list2[i], d1y);
                double D2 = distanceCalculation(list1[i], d2x, list2[i], d2y);
                double D3 = distanceCalculation(list1[i], d3x, list2[i], d3y);
                dataGridView1.Rows[0].Cells[i].Value = D1;
                dataGridView1.Rows[1].Cells[i].Value = D2;
                dataGridView1.Rows[2].Cells[i].Value = D3;

                
                consoleTextBox.AppendText("D(C" + (i + 1) + ",P1) = " + D1 + "\n");
                consoleTextBox.AppendText("D(C" + (i + 1) + ",P3) = " + D2 + "\n");
                consoleTextBox.AppendText("D(C" + (i + 1) + ",P3) = " + D3 + "\n");

                if (D1 < D2 && D1 < D3)
                {
                    consoleTextBox.AppendText("Cluster = 1\n");
                    clusterObjects11.Add(list1[i]);
                    clusterObjects12.Add(list2[i]);
                    dataGridView1.Rows[3].Cells[i].Value = 1;
                    clusterlist2.Add(1);
                }
                else if (D2 < D1 && D2 < D3)
                {
                    consoleTextBox.AppendText("Cluster = 2\n");
                    clusterObjects21.Add(list1[i]);
                    clusterObjects22.Add(list2[i]);
                    dataGridView1.Rows[3].Cells[i].Value = 2;
                    clusterlist2.Add(2);

                }
                else if (D3 < D1 && D3 < D2)
                {
                    consoleTextBox.AppendText("Cluster = 3\n");
                    clusterObjects31.Add(list1[i]);
                    clusterObjects32.Add(list2[i]);
                    dataGridView1.Rows[3].Cells[i].Value = 3;
                    clusterlist2.Add(3);
                }

            }
            if (clusterlist1.SequenceEqual(clusterlist2))
            {
                isResult = true;
                performBtn.Text = "Done";

                consoleTextBox.AppendText("\nThe current clusters are the same as the previous ones\n");
                consoleTextBox.AppendText("The results of subdivision:\n");

                consoleTextBox.AppendText("\nThe first cluster:\n");
                for (int i = 0; i < list1.Count; i++) {
                    if (clusterlist1[i] == 1) {
                        
                        consoleTextBox.AppendText("C"+ (i + 1) + ", ");
                    }
                }
                
                consoleTextBox.AppendText("\nThe second cluster:\n");
                for (int i = 0; i < list1.Count; i++)
                {
                    if (clusterlist1[i] == 2)
                    {
                        consoleTextBox.AppendText("C" + (i + 1) + ", ");
                    }
                }
                consoleTextBox.AppendText("\nThe third cluster:\n");
                for (int i = 0; i < list1.Count; i++)
                {
                    if (clusterlist1[i] == 3)
                    {
                        consoleTextBox.AppendText("C" + (i + 1) + ", ");
                    }
                }
                MessageBox.Show("Done!");
                consoleTextBox.AppendText("\nDone");
            }
            else {
                clusterlist1 = clusterlist2;
            }

        }

        public bool compareLists(List<double> list1, List<double> list2) {
            if (clusterlist1.SequenceEqual(clusterlist2)) {
                return true;
            }
            return false;        
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {
            

        }
        
        //exit app
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //open file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "TXT files|*.txt";

            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = theDialog.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            textInput1.Text = theDialog.FileName;
                            var lines = File.ReadAllLines(theDialog.FileName);
                            // read the first two lines and split by comma then convert to double
                             list1 = lines[0].Split(',').ToList<string>().Select(x => double.Parse(x)).ToList();
                             list2 = lines[1].Split(',').ToList<string>().Select(x => double.Parse(x)).ToList();

                            listMaximin1 = lines[0].Split(',').ToList<string>().Select(x => double.Parse(x)).ToList();
                            listMaximin2 = lines[1].Split(',').ToList<string>().Select(x => double.Parse(x)).ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
        public void reset()
        {


        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextWriter oldOut = Console.Out;

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "txt files (*.txt)|*.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, consoleTextBox.Text);
                }
            }
            
        }

        private void algorithmcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            performBtn.Text = "Start";
            if (algorithmcomboBox.SelectedIndex == 0)
            {
                groupsComboBox.Visible = true;
                rateComboBox.Visible = true;
                groupsLabel.Visible = true;
                rateLabel.Visible = true;
            }
            else {
                groupsComboBox.Visible = false;
                rateComboBox.Visible = false;
                groupsLabel.Visible = false;
                rateLabel.Visible = false;
            }
        }

      
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void dataGridView1_ColumnHeaderCellChanged(object sender, DataGridViewColumnEventArgs e)
        {

        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.Show();
        }

        private void consoleTextBox_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            consoleTextBox.SelectionStart = consoleTextBox.Text.Length;
            // scroll it automatically
            consoleTextBox.ScrollToCaret();
        }

        private void consoleTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu cm = new ContextMenu();//make a context menu instance



                MenuItem item = new MenuItem();//make a menuitem instance
                item.Text = "Clear Console";//give the item a header
                item.Click += Item_Click; ;//give the item a click event handler
                cm.MenuItems.Add(item);//add the item to the context menu

                MenuItem item2 = new MenuItem();//make a menuitem instance
                item2.Text = "Save as Text File";//give the item a header
                item2.Click += Item2_Click; ; ;//give the item a click event handler
                cm.MenuItems.Add(item2);//add the item to the context menu

                ((RichTextBox)sender).ContextMenu = cm;//add the context menu to the sender
                cm.Show(consoleTextBox, e.Location);//show the context menu
            }
            
        }

        private void Item2_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "txt files (*.txt)|*.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, consoleTextBox.Text);
                }
            }
        }

        private void Item_Click(object sender, EventArgs e)
        {
            consoleTextBox.Text = "";
        }

        public void clearConsole() {
        
        
        }
    }
    public class clusterListValues
    {

        private double valueNumber;
        private int clusterNumber;

        public clusterListValues(double valueNumber, int clusterNumber)
        {
            this.valueNumber = valueNumber;
            this.clusterNumber = clusterNumber;
        }
        public double ValueNumber
        {
            get { return valueNumber; }
            set { valueNumber = value; }
        }

        public int ClusterNumber
        {
            get { return clusterNumber; }
            set { clusterNumber = value; }
        }
    }
}
