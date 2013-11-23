using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace HoptServer.Models
{
    public class CalculateCosts
    {
        public double constructionCost(CostInfo costInfo, RoomType[] rooms) {
            double value = 0;
            for (var i = 0; i < 6; i++) {
                value += costInfo.capital[i].construction * (rooms[i].num - rooms[i].originalNum);
            }
            return value;
        }
        public double equipmentCost(CostInfo costInfo, RoomType[] rooms)
        {
            double value = 0;
            for (var i = 0; i < 6; i++) {
                value += costInfo.capital[i].equipment * (rooms[i].num - rooms[i].originalNum);
            }
            return value;
        }
        public double initialCost(CostInfo costInfo, RoomType[] rooms)
        {
            return constructionCost(costInfo, rooms) + equipmentCost(costInfo, rooms);
        }
        public double annualCost(CostInfo costInfo, RoomType[] rooms, Response[] acuityInfo, Response[] arrivalInfo)
        {
            return utilityCost(costInfo, rooms) + staffCost(costInfo, rooms, getUtilizationAndLWBS(rooms)) + lwbsCost(acuityInfo, arrivalInfo, getUtilizationAndLWBS(rooms));
        }
        public double utilityCost(CostInfo costInfo, RoomType[] rooms)
        {
            double value = 0;
            for (var i = 0; i < 6; i++) {
                value += costInfo.utility.value * rooms[i].num * costInfo.capital[i].sqft;
            }
            return value;
        }
        //TODO: utilization - (from Simio)
        public double staffCost(CostInfo costInfo, RoomType[] rooms, double[] simulationResponses)
        {
            double value = 0;
            for (var i = 0; i < 3; i++) {
                for (var j = 0; j < 6; j++) {
                    double utilization, ratio = 0;
                    //if (j == 0) { utilization = simulationResponses[j]/100; } //ExamRoomUtilization/100; }
                    //else if (j == 1) { utilization = simulationResponses[j]/100; } //TraumaUtilization/100; }
                    //else if (j == 2) { utilization = simulationResponses[j]/100; } //FastTrackUtilization/100; }
                    //else if (j == 3) { utilization = simulationResponses[j]/100; } //RapidAdmissionUnitUtilization/100; }
                    //else if (j == 4) { utilization = simulationResponses[j]/100; } //BehavioralUtilization/100; }
                    //else if (j == 5) { utilization = simulationResponses[j]/100; } //ObservationUtilization/100; }

                    utilization = simulationResponses[j]/100;

                    if (i == 2 && j == 3) { ratio = 0; }
                    else if (i == 2 && j == 4) { ratio = 0; }
                    else if (i == 2 && j == 5) { ratio = 0; }
                    else { ratio = 1/costInfo.labor[i].rooms[j].value; }

                    // console.log(i,j,rooms[j].num,hospitalData.costInfo.labor[i].wage,utilization,ratio);

                    value += rooms[j].num * costInfo.labor[i].wage * utilization * ratio;
                }
            }
            return value;
        }

        public double[] getUtilizationAndLWBS(RoomType[] rooms)
        {
            double[] values = new double[7];
            SQLiteConnection conn = new SQLiteConnection("Data Source = configs.db");
            conn.Open();
            String sql = "select * from Results where ";
            for (int i = 0; i < 6; i++)
            {
                if(i < 5)
                    sql += rooms[i].name.Replace(" ", "") + " = " + rooms[i].num + " and ";
                else
                    sql += rooms[i].name.Replace(" ", "") + " = " + rooms[i].num;
            }
            Console.Write(sql);
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataReader dr = cmd.ExecuteReader();
            while(dr.Read())
            {
                values[0] = Convert.ToDouble(dr["ExamRoomUtilization"]);
                values[1] = Convert.ToDouble(dr["TraumaUtilization"]);
                values[2] = Convert.ToDouble(dr["FastTrackUtilization"]);
                values[3] = Convert.ToDouble(dr["RapidAdmissionUnitUtilization"]);
                values[4] = Convert.ToDouble(dr["BehavioralUtilization"]);
                values[5] = Convert.ToDouble(dr["ObservationUtilization"]);
                values[6] = Convert.ToDouble(dr["LWBS"]);
            }
            return values;

        }
        //TODO: revenue by acuity
        //TODO: lwbs (from Simio)
        public double lwbsCost(Response[] acuityInfo, Response[] arrivalInfo, double[] simulationResponses) {
            double value = 0;
                for (int i = 0; i < 5; i++) {
                    value += 365 * acuityInfo[i].value / 100 * arrivalInfo[2].value * (simulationResponses[6]);
                }
            return value;
        }
        //value at construction start
        public double costAtConstructionStart(CostInfo costInfo, RoomType[] rooms, Response[] acuityInfo, Response[] arrivalInfo, double interestRate, double growthRate, int yearsToCompletion, int yearsAhead) {
            double annuityOfAnnualCost = annualCost(costInfo, rooms, acuityInfo, arrivalInfo) * ((1 - Math.Pow((1 + growthRate) / (1 + interestRate), yearsAhead)) / ((interestRate - growthRate) * Math.Pow(1 + interestRate, yearsToCompletion)));
            return initialCost(costInfo,rooms) + annuityOfAnnualCost;
        }
    }
}
