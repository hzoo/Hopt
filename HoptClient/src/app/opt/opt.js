angular.module( 'ngBoilerplate.opt', [
  'ui.router',
  'ngGrid'
])

.config(function config( $stateProvider ) {
  $stateProvider.state( 'opt', {
    url: '/opt',
    views: {
      "main": {
        controller: 'OptCtrl',
        templateUrl: 'opt/opt.tpl.html'
      }
    },
    data:{ pageTitle: 'Optimization' }
  });
})

.factory('signalRSvc2', function signalRSvc($, $rootScope) {
    return {
        proxy: null,
        initialize: function (responseCallback, connectionCallback, port, hub) {
            if (this.proxy === null) {
              //Getting the connection object
              if (port === 8001) {
                connection = $.hubConnection('http://localhost:' + port);
              } else {
                connection = $.hubConnection('http://edtoolserver.ngrok.com');
              }
              //Creating proxy
              this.proxy = connection.createHubProxy(hub);
              //Starting connection
              connection.start({ jsonp: true }).done(function () {
                $rootScope.$apply(function () {
                      connectionCallback();
                  });
              });
              //Attaching a callback to handle client call
              this.proxy.on('getResponses', function (message) {
                // console.log(message);
                // console.log(responseCallback);
                  $rootScope.$apply(function () {
                      responseCallback(message);
                  });
              });

              connection.stateChanged(function (change) {
                  if (change.newState === $.signalR.connectionState.reconnecting) {
                      console.log("Hopt is reconnecting!");
                  }
                  else if (change.newState === $.signalR.connectionState.connected) {
                      console.log("Hopt is connected!");
                  }
                  else if (change.newState === $.signalR.connectionState.connecting) {
                      console.log("Hopt is connecting!");
                  }
                  else if (change.newState === $.signalR.connectionState.disconnected) {
                      console.log("Hopt is disconnected!");
                  }
              });
          }
        },
        sendRequest: function (method, param1) {
          // console.log('sendRequest1');
          this.proxy.invoke(method, param1);
        },
        sendRequest2: function (method, param1, param2) {
          // console.log('sendRequest2',method,param1,param2);
          this.proxy.invoke(method, param1, param2);
        }
    };
})

.controller('OptCtrl', function OptCtrl( $scope, signalRSvc2, HospitalData, HoptService, Configuration, $filter ) {
  // $scope.hospitalData = HospitalData.getHospitalData();
  $scope.hospitalData = HospitalData.hospitalData;
  $scope.hoptService = HoptService.getHoptService();
  $scope.configuration = Configuration.getConfiguration();
  $scope.cost = {
    initial: {},
    annual: {}
  };

  $scope.cost.initial.construction = function() {
    var value = 0;
    for (var i = 0; i < 6; i++) {
      // console.log($scope.hospitalData.costInfo.capital[i].construction,$scope.configuration.rooms[i].num);
      value += $scope.hospitalData.costInfo.capital[i].construction * ($scope.configuration.rooms[i].num);// - $scope.configuration.rooms[i].originalNum);
    }
    return value;
  };
  $scope.cost.initial.equipment = function() {
    var value = 0;
    for (var i = 0; i < 6; i++) {
      // console.log($scope.hospitalData.costInfo.capital[i].equipment,$scope.configuration.rooms[i].num);
      value += $scope.hospitalData.costInfo.capital[i].equipment * ($scope.configuration.rooms[i].num);// - $scope.configuration.rooms[i].originalNum);
    }
    return value;
  };
  $scope.cost.initial.total = function() {
    return $scope.cost.initial.construction() + $scope.cost.initial.equipment();
  };
  $scope.cost.annual.total = function() {
    if ($scope.hoptService.responses[0]) {
      // console.log($scope.cost.annual.utility(),$scope.cost.annual.staff(),$scope.cost.annual.lwbs());
      return $scope.cost.annual.utility() + $scope.cost.annual.staff() + $scope.cost.annual.lwbs();
    } else {
      return "Calculated after running the simulation";
    }
  };
  $scope.cost.annual.utility = function() {
    var value = 0;
    if ($scope.hoptService.responses[0]) {
      for (var i = 0; i < 6; i++) {
        // console.log(i,$scope.hospitalData.costInfo.utility.value,$scope.configuration.rooms[i].num,$scope.hospitalData.costInfo.capital[i].sqft);
        value += $scope.hospitalData.costInfo.utility.value * $scope.configuration.rooms[i].num * $scope.hospitalData.costInfo.capital[i].sqft;
      }
    } else {
      return "Calculated after running the simulation";
    }
    return value;
  };
  //TODO: utilization - (from Simio)
  $scope.cost.annual.staff = function() {
    var value = 0;
    if ($scope.hoptService.responses[0]) {
      for (var i = 0; i < 3; i++) {
        for (var j = 0; j < 6; j++) {
          var utilization, ratio = 0;
          if (j === 0) { utilization = $scope.hoptService.responses[0].ExamRoomUtilization/100; }
          else if (j === 1) { utilization = $scope.hoptService.responses[0].TraumaUtilization/100; }
          else if (j === 2) { utilization = $scope.hoptService.responses[0].FastTrackUtilization/100; }
          else if (j === 3) { utilization = $scope.hoptService.responses[0].RapidAdmissionUnitUtilization/100; }
          else if (j === 4) { utilization = $scope.hoptService.responses[0].BehavioralUtilization/100; }
          else if (j === 5) { utilization = $scope.hoptService.responses[0].ObservationUtilization/100; }

          if (i === 2 && j === 3) { ratio = 0; }
          else if (i === 2 && j === 4) { ratio = 0; }
          else if (i === 2 && j === 5) { ratio = 0; }
          else { ratio = 1/$scope.hospitalData.costInfo.labor[i].rooms[j].value; }

          // console.log(i,j,$scope.configuration.rooms[j].num,$scope.hospitalData.costInfo.labor[i].wage,utilization,ratio);

          value += $scope.configuration.rooms[j].num * $scope.hospitalData.costInfo.labor[i].wage *
          // $scope.hospitalData.serviceInfo[j].utilization / $scope.hospitalData.costInfo.labor[i].rooms[j].value;
          utilization * ratio;
        }
      }
    } else {
      value = "Calculated after running the simulation";
    }
    return value;
  };
  //TODO: revenue by acuity
  //TODO: lwbs (from Simio)
  $scope.cost.annual.lwbs = function() {
    var value = 0;
    if ($scope.hoptService.responses[0]) {
      for (var i = 0; i < 5; i++) {
      // console.log($scope.hospitalData.acuityInfo[i].value,$scope.hospitalData.arrivalInfo[2].value,Number($scope.hoptService.responses[0].LWBS));
       value += 365 * $scope.hospitalData.acuityInfo[i].value / 100 * $scope.hospitalData.arrivalInfo[2].value * Number($scope.hoptService.responses[0].LWBS);
      }
    } else {
      return "Calculated after running the simulation";
    }
    return value;
  };
  //value at construction start
  $scope.cost.total = function(interestRate,growthRate,yearsToCompletion) {
    if ($scope.hoptService.responses[0]) {
      // console.log($scope.cost.annual.total(),(interestRate-growthRate),Math.pow(1+interestRate/100, yearsToCompletion));
      var annuityOfAnnualCost = $scope.cost.annual.total() * ((1-Math.pow((1+ growthRate)/( 1+ interestRate), 10))/ ((interestRate-growthRate) * Math.pow(1+interestRate, yearsToCompletion)));
      return $scope.cost.initial.total() + annuityOfAnnualCost;
    } else {
      return "Calculated after running the simulation";
    }

  };

  $scope.misc = {};
  $scope.misc.viewLoading = false;
  $scope.runOpt = false;

  $scope.config = {
      daysToRun: $scope.hospitalData.simulationInfo.daysToSimulate,
      numberOfReps: $scope.hospitalData.simulationInfo.numberOfReplications,
      startupTime: $scope.hospitalData.simulationInfo.startupTime,
      rateTable: $scope.hospitalData.simulationInfo.rateTable,
      arrivalInfo: $scope.hospitalData.arrivalInfo,
      acuityInfo: $scope.hospitalData.acuityInfo,
      serviceInfo: $scope.hospitalData.serviceInfo,
      constraintInfo: $scope.hospitalData.constraintInfo,
      costInfo: $scope.hospitalData.costInfo,
      rooms: $scope.configuration.rooms
  };

  $scope.gridOptions = {
    data: 'hoptService.responses',
    showGroupPanel: false,
    showFilter: true,
    showColumnMenu: true,
    enableColumnResize: true
  };

  $scope.misc.responses = $scope.hoptService.lastResponses;

  updateConfigResponses = function (data) {
    //check if response or obj (response/config)
    // console.log(data);
    $scope.misc.viewLoading = false;
    var obj = {
      TimeinSystem: '',
      AvgNumberinWaitingRoom: '',
      ExamRoomUtilization: '',
      TraumaUtilization: '',
      FastTrackUtilization: '',
      RapidAdmissionUnitUtilization: '', //why unit?
      BehavioralUtilization: '', //spelling
      ObservationUtilization: '',
      AvgWaitingTime: '',
      LWBS: '',
      TotalPeople: ''
    };

    angular.forEach(data, function(value, key) {
      if (value.name == 'LWBS') {
        obj[value.name] =  value.value;
      } else {
        obj[value.name] = $filter('number')(value.value, 2);
      }

      for (var j = 0; j < $scope.misc.responses.length; j++) {
        if ($scope.misc.responses[j].name === value.name) {
          if ($scope.misc.responses[j].value === '' || $scope.misc.responses[j].value === undefined) {
            $scope.misc.responses[j].diff = '';
          } else {
            $scope.misc.responses[j].diff = $filter('number')(value.value - $scope.misc.responses[j].value, 3);
          }

          $scope.misc.responses[j].value = $filter('number')(value.value, 2);
      }
      }
    });

    angular.forEach(obj, function(value, key) {
        // console.log(key + ': ' + value + '.');
      if (value === '') {
        // console.log( + " is blank");
        for (var j = 0; j < $scope.misc.responses.length; j++) {
          if ($scope.misc.responses[j].name === key) {
            $scope.misc.responses[j].diff = '';
            $scope.misc.responses[j].value = '';
          }
        }
      }
    });

    $scope.hoptService.responses.unshift(obj);

    //run next optimization
    // console.log('requesting next config');
    // console.log($scope.runOpt);
    // if ($scope.runOpt === true) {

    //   roomIterator[0]++;
    //   var nextRoomNum = Number($scope.configuration.rooms[0].originalNum) + roomIterator[0]*defaultStaffingRatios[0][0];
    //   console.log($scope.configuration.rooms[0].originalNum,roomIterator[0],defaultStaffingRatios[0][0],nextRoomNum, $scope.hospitalData.constraintInfo.rooms[0].maximumNumberOfRooms);
    //   if (nextRoomNum < $scope.hospitalData.constraintInfo.rooms[0].maximumNumberOfRooms) {
    //     $scope.configuration.rooms[0].num = nextRoomNum;
    //     document.getElementById('room0').value = $scope.configuration.rooms[0].num;
    //     signalRSvc2.sendRequest2('ReturnNextConfig', $scope.config, $scope.cost.total(0.05,0.04,5));
    //   } else {
    //     $scope.runOpt = false;
    //   }
    // }

  };

  $scope.stopOpt = function() {
    $scope.runOpt = false;
  };

  $scope.fixBlank = function(value,filter) {
    if (value === '') {
      return '--';
    } else {
      if (value == "Calculated after running the simulation") { return value; }
      else if (filter == "$") { return $filter('currency')(value, '$'); }
      else { return value; }
    }
  };

  $scope.fixDiff = function(value) {
    if (value === '' || value === "0.000" || value === "-0.000") {
      return '--';
    } else {
      return value;
    }
  };

  var submitClicked = false;
  sendMessage = function() {
    if (submitClicked === true) {
      signalRSvc2.sendRequest('RunConfig',$scope.config);
    }
  };

  $scope.reInitialize = function() {
    signalRSvc2.proxy = null;
    signalRSvc2.initialize(updateConfigResponses, sendMessage, $scope.hoptService.portNumber, 'OptHub');
  };

  var roomIterator;

  /*
   * @param {string} method Method Name
   * @param {object} message Data to send
   */
  $scope.sendMessage = function (method, message) {

    $scope.configuration.rooms[0].num = document.getElementById('room0').value;
    $scope.configuration.rooms[1].num = document.getElementById('room1').value;
    $scope.configuration.rooms[2].num = document.getElementById('room2').value;
    $scope.configuration.rooms[3].num = document.getElementById('room3').value;
    $scope.configuration.rooms[4].num = document.getElementById('room4').value;
    $scope.configuration.rooms[5].num = document.getElementById('room5').value;

    $scope.configuration.rooms[0].originalNum = document.getElementById('originalRoom0').value;
    $scope.configuration.rooms[1].originalNum = document.getElementById('originalRoom1').value;
    $scope.configuration.rooms[2].originalNum = document.getElementById('originalRoom2').value;
    $scope.configuration.rooms[3].originalNum = document.getElementById('originalRoom3').value;
    $scope.configuration.rooms[4].originalNum = document.getElementById('originalRoom4').value;
    $scope.configuration.rooms[5].originalNum = document.getElementById('originalRoom5').value;

    // roomIterator = [0,0,0,0,0,0];
    // $scope.runOpt = true;

    $scope.misc.viewLoading = true;
    submitClicked = true;
    // console.log('sending message');
    if (signalRSvc2.proxy !== null) {
      console.log('signalRSvc2 isInitialized');
      signalRSvc2.sendRequest(method, message);
    } else {
      console.log('signalRSvc2 not isInitialized');
      signalRSvc2.initialize(updateConfigResponses, sendMessage, $scope.hoptService.portNumber, 'OptHub');
    }
  };

  signalRSvc2.initialize(updateConfigResponses, sendMessage, $scope.hoptService.portNumber, 'OptHub');
})

;