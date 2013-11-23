ngBoilerplate.service('HoptService', function HoptService() {
  var hoptService = {};
  hoptService.responses = [];
  hoptService.portNumber = "8001";
  hoptService.lastResponses = [
      { displayName: "Avg.  # Waiting", name: "AvgNumberinWaitingRoom", value: '', diff: '' },
      { displayName: "Avg. Waiting Time", name: "AvgWaitingTime", value: '', diff: '' },
      { displayName: "Avg. Waiting Exam Room", name: "WaitingTimeForER", value: '', diff: '' },
      { displayName: "Avg. Waiting Trauma", name: "WaitingTimeForTrauma", value: '', diff: '' },
      { displayName: "Avg. Waiting Fast Track", name: "WaitingTimeForFT", value: '', diff: '' },
      { displayName: "Exam Room", name: "ExamRoomUtilization", value: '', diff: '' },
      { displayName: "Trauma", name: "TraumaUtilization", value: '', diff: '' },
      { displayName: "Fast Track", name: "FastTrackUtilization", value: '', diff: '' },
      { displayName: "Rapid Admission", name: "RapidAdmissionUnitUtilization", value: '', diff: '' }, //why unit?
      { displayName: "Behavioral", name: "BehavioralUtilization", value: '', diff: '' }, //spelling
      { displayName: "Observation", name: "ObservationUtilization", value: '', diff: '' },
      { displayName: "Time in System", name: "TotalTimeOfStay", value: '', diff: '' },
      { displayName: "LWBS", name: "LWBS", value: '', diff: '' },
      { displayName: "Total Visits", name: "TotalVisits", value: '', diff: '' }
    ];
  return {
     getHoptService: function() {
      return hoptService;
    }
  };
});

ngBoilerplate.service('Configuration', function Configuration() {
  var configuration = {};
  configuration.rooms = [
    {
      id: 0,
      name: "Exam Room",
      num: 37,
      originalNum: 37,
      included: true
    },
    {
      id: 1,
      name: "Trauma",
      num: 0,
      originalNum: 0,
      included: true
    },
    {
      id: 2,
      name: "Fast Track",
      num: 8,
      originalNum: 8,
      included: true
    },
    {
      id: 3,
      name: "Rapid Admission",
      num: 0,
      originalNum: 0,
      included: true
    },
    {
      id: 4,
      name: "Behavioral",
      num: 14,
      originalNum: 14,
      included: true
    },
    {
      id: 5,
      name: "Observation",
      num: 14,
      originalNum: 14,
      included: true
    }
  ];

  return {
     getConfiguration: function() {
      return configuration;
    }
  };
});

ngBoilerplate.service('HospitalData', function HospitalData() {
//defaults
  defaultStaffingRatios = [];

  //Nurse
  defaultStaffingRatios[0] = [];
    //exam room
    defaultStaffingRatios[0][0] = 4;//1/4;
    //trauma
    defaultStaffingRatios[0][1] = 1;//1/1; // 2/1 for the first hour, then 1/1
    //fast track
    defaultStaffingRatios[0][2] = 6;
    //rapid admission
    defaultStaffingRatios[0][3] = 6; //1/6;
    //behavioral
    defaultStaffingRatios[0][4] = 4; //1/4;
    //observation
    defaultStaffingRatios[0][5] = 6; //1/6;
    //triage
    // defaultStaffingRatios[0][3] = 1;//1/1; // only for time in triage, < 5 min

  //Technician
  defaultStaffingRatios[1] = [];
    //exam room
    defaultStaffingRatios[1][0] = 6;//1/6;
    //trauma
    defaultStaffingRatios[1][1] = 1;//1/1;
    //fast track
    defaultStaffingRatios[1][2] = 6;//1/6;
    //rapid admission
    defaultStaffingRatios[1][3] = 6; //1/6;
    //behavioral
    defaultStaffingRatios[1][4] = 4; //1/4;
    //observation
    defaultStaffingRatios[1][5] = 6; //1/6;
    //triage
    // defaultStaffingRatios[1][3] = 1;//1/1; // only for time in triage, < 5 min

  //Doctor
  defaultStaffingRatios[2] = [];
    //exam room
    defaultStaffingRatios[2][0] = 8;//1/8;
    //trauma
    defaultStaffingRatios[2][1] = 1;//1/1;
    //fast track
    defaultStaffingRatios[2][2] = 6;//1/6;
    defaultStaffingRatios[2][3] = 1; //actually 0/1;
    defaultStaffingRatios[2][4] = 1; //actually 0/1;
    defaultStaffingRatios[2][5] = 1; //actually 0/1;
    //triage
    // defaultStaffingRatios[2][3] = 1;//actually 0/1;

    var defaultUtilityCost = 4; //$4/sq ft

    var defaultAverageRoomTime = [];
    defaultAverageRoomTime[0] = "Random.Triangular(1.8,3.2,4.6)"; //this is actual total length of stay 6
    defaultAverageRoomTime[1] = 2; //this is actual total length of stay 3
    defaultAverageRoomTime[2] = 1.2; //this is actual total length of stay 5/6
    defaultAverageRoomTime[3] = 2.8; //this is actual total length of stay 16
    defaultAverageRoomTime[4] = 9.5; //this is actual total length of stay 24
    defaultAverageRoomTime[5] = 5.6; //this is actual total length of stay 16

    var defaultWage = [];
    defaultWage[0] = 62060; //nurse
    defaultWage[1] = 31752; //tech
    defaultWage[2] = 287859; //doctor

    var defaultConstructionCost = [];
    defaultConstructionCost[0] = 224000;
    defaultConstructionCost[1] = 336000;
    defaultConstructionCost[2] = 179200; //80% of Exam Room
    defaultConstructionCost[3] = 224000; //same as Exam Room
    defaultConstructionCost[4] = 246400; //110% of Exam Room
    defaultConstructionCost[5] = 224000; //same as Exam Room

    var defaultEquipmentCost = [];
    defaultEquipmentCost[0] = 43437;
    defaultEquipmentCost[1] = 103030;
    defaultEquipmentCost[2] = 34749.6; //80% of Exam Room
    defaultEquipmentCost[3] = 43437; //same as Exam Room
    defaultEquipmentCost[4] = 47780; //110% of Exam Room
    defaultEquipmentCost[5] = 43437;//same as Exam Room

    var defaultIsIncluded = [];
    defaultIsIncluded[0] = true;
    defaultIsIncluded[1] = true;
    defaultIsIncluded[2] = true;
    defaultIsIncluded[3] = true;
    defaultIsIncluded[4] = true;
    defaultIsIncluded[5] = true;

    var defaultSquareFeet = [];
    defaultSquareFeet[0] = 800; //renovating existing ED
    defaultSquareFeet[1] = 1200; //renovating existing ED
    defaultSquareFeet[2] = 800; //renovating existing ED
    defaultSquareFeet[3] = 800; //renovating existing ED
    defaultSquareFeet[4] = 800; //renovating existing ED
    defaultSquareFeet[5] = 800; //renovating existing ED

    var defaultMaxNumberofRooms = [];
    defaultMaxNumberofRooms[0] = 150;
    defaultMaxNumberofRooms[1] = 10;
    defaultMaxNumberofRooms[2] = 100;
    defaultMaxNumberofRooms[3] = 40; //3 and 5 actually both add to 40
    defaultMaxNumberofRooms[4] = 30;
    defaultMaxNumberofRooms[5] = 40;

  //categorize the types of hospital data
  hospitalDataTypes = [
    "clientInfo",
    "arrivalInfo",
    "acuityInfo",
    "serviceInfo",
    "constraintInfo",
    "costInfo",
    "simulationInfo"
  ];

  possibleRoomTypesNames = [
    "Exam Room",
    "Trauma",
    "Fast Track",
    "Rapid Admission",
    "Behavioral",
    "Observation"
  ];

  possibleStaffTypesNames = [
    "Nurse",
    "Technician",
    "Doctor"
  ];

  var hospitalData = {};

  //add to the data object
  hospitalData.clientInfo = [
    { name: "Hospital Name", value: "Hospital X" },
    { name: "Hospital Location", value: "" }];

  hospitalData.arrivalInfo = [
    { name: "Annual Visits", value: 134924 },
    { name: "% of Annual Visits for the Peak Month", value: 10 },
    { name: "LWBS Revenue", value: 100 }];

  hospitalData.acuityInfo = [
    { name: "Acuity 1 %", value: 0.555, revenue: 100 }, //revenue is fake 0.52
    { name: "Acuity 2 %", value: 10.214, revenue: 100 }, //revenue is fake 9.47
    { name: "Acuity 3 %", value: 56.069, revenue: 100 }, //revenue is fake 58
    { name: "Acuity 4 %", value: 30.535, revenue: 100 }, //revenue is fake 29.8
    { name: "Acuity 5 %", value: 2.202, revenue: 100 }]; //revenue is fake 1.6

  hospitalData.simulationInfo = {
    daysToSimulate: { name: "Days to Simulate", value: 14 },
    numberOfReplications: { name: "# of Replications", value: 1 },
    startupTime: { name: "Startup Time (hrs)", value: 168 },
    rateTable: { name: "Rate Table to run", value: "average" }};

  //main ed is never excluded
  hospitalData["serviceInfo"] = [];
  for (var j = 0; j < possibleRoomTypesNames.length; j++) {
    hospitalData.serviceInfo[j] = {
      name: possibleRoomTypesNames[j],
      averageRoomTime: defaultAverageRoomTime[j],
      included: defaultIsIncluded[j]
    };
  }

  //constraints on maximum waiting time for each type
  hospitalData.constraintInfo = {
    rooms: []
  };

  for (var k = 0; k < possibleRoomTypesNames.length; k++) {
    hospitalData.constraintInfo.rooms[k] = {
      name: possibleRoomTypesNames[k],
      averageWaitTime: 0,
      maximumNumberOfRooms: defaultMaxNumberofRooms[k]
    };
  }

  //costs object
  hospitalData["costInfo"] = {
    capital : [],
    labor: [],
    utility: { name: "Average Cost of Utility / Square Feet", value: defaultUtilityCost }
  };

  //add costs for each type of room
  for (var a = 0; a < possibleRoomTypesNames.length; a++) {
    hospitalData.costInfo.capital[a] = {
      name: possibleRoomTypesNames[a],
      equipment: defaultEquipmentCost[a],
      construction: defaultConstructionCost[a],
      sqft: defaultSquareFeet[a]
    };
  }

  //add costs for each type of staff
  for (var b = 0; b < possibleStaffTypesNames.length; b++) {
      hospitalData.costInfo.labor[b] = {
        name: possibleStaffTypesNames[b],
        wage: defaultWage[b],
        rooms: []
      };
      for (var c = 0; c < possibleRoomTypesNames.length; c++) {
        hospitalData.costInfo.labor[b].rooms[c] = {
          name: possibleRoomTypesNames[c],
          value: defaultStaffingRatios[b][c]
        };
      }
  }

  return {
    //  getHospitalData: function() {
    //   return hospitalData;
    // },
    hospitalData: hospitalData

  };

});