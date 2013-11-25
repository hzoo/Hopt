angular.module( 'ngBoilerplate.config', [
  'ui.router',
  'ngGrid'
])

.config(function config( $stateProvider ) {
  $stateProvider.state( 'config', {
    url: '/config',
    views: {
      "main": {
        controller: 'ConfigCtrl',
        templateUrl: 'config/config.tpl.html'
      }
    },
    data:{ pageTitle: 'Run Configurations' }
  });
})

.value('$',$)

.factory('signalRSvc', function signalRSvc($, $rootScope) {
    return {
        proxy: null,
        initialize: function (responseCallback, connectionCallback, port, hub) {
            if (this.proxy === null) {
              // console.log(connectionCallback,port,hub);
              //Getting the connection object
              if (port === '8001') {
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
                console.log(message);
                // console.log(responseCallback);
                  $rootScope.$apply(function () {
                      responseCallback(message);
                  });
              });

              connection.stateChanged(function (change) {
                  // signalR.connectionState = {
                  //     connecting: 0,
                  //     connected: 1,
                  //     reconnecting: 2,
                  //     disconnected: 4
                  // };

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
        sendRequest: function (method, message) {
          this.proxy.invoke(method, message);
        }
    };
})

.controller('ConfigCtrl', function ConfigCtrl( $scope, signalRSvc, HospitalData, HoptService, Configuration, $filter ) {
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
      value += $scope.hospitalData.costInfo.capital[i].construction * ($scope.configuration.rooms[i].num - $scope.configuration.rooms[i].originalNum);
    }
    return value;
  };
  $scope.cost.initial.equipment = function() {
    var value = 0;
    for (var i = 0; i < 6; i++) {
      // console.log($scope.hospitalData.costInfo.capital[i].equipment,$scope.configuration.rooms[i].num);
      value += $scope.hospitalData.costInfo.capital[i].equipment * ($scope.configuration.rooms[i].num - $scope.configuration.rooms[i].originalNum);
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
        // console.log(Number($scope.hospitalData.daysToSimulate),$scope.hospitalData.acuityInfo[i].value,$scope.hospitalData.arrivalInfo[2].value,Number($scope.hoptService.responses[0].LWBS));
       value += 365 * Number($scope.hospitalData.daysToSimulate) * $scope.hospitalData.acuityInfo[i].value / 100 * $scope.hospitalData.arrivalInfo[2].value * Number($scope.hoptService.responses[0].LWBS);
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
  $scope.misc.runButton = 'Run';
  $scope.misc.viewLoading = false;

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
    console.log(data);
    $scope.misc.viewLoading = false;
    var obj = {
      AvgNumberinWaitingRoom: '',
      AvgWaitingTime: '',
      WaitingTimeForER: '',
      WaitingTimeForTrauma: '',
      WaitingTimeForFT: '',
      ExamRoomUtilization: '',
      TraumaUtilization: '',
      FastTrackUtilization: '',
      RapidAdmissionUnitUtilization: '',
      BehavioralUtilization: '',
      ObservationUtilization: '',
      TotalTimeOfStay: '',
      LWBS: '',
      TotalVisits: ''
    };

    angular.forEach(data, function(value, key) {
      if (value.name == 'LWBS' || value.name == 'TotalVisits') {
        obj[value.name] = value.value;
      } else {
        obj[value.name] = $filter('number')(value.value, 2);
      }

      for (var j = 0; j < $scope.misc.responses.length; j++) {
        if ($scope.misc.responses[j].name === value.name) {
          if ($scope.misc.responses[j].value === '' || $scope.misc.responses[j].value === undefined) {
            $scope.misc.responses[j].diff = '';
          } else {
            $scope.misc.responses[j].diff = value.value - $scope.misc.responses[j].value;
          }

          $scope.misc.responses[j].value = value.value;
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
    $scope.misc.runButton = 'Run';

    //signalRSvc.sendRequest(method, message);

  };

  $scope.fixBlank = function(value,filter,num) {
    if (value === '') {
      return '--';
    } else {
      if (value == "Calculated after running the simulation") { return value; }
      else if (filter == "$") { return $filter('currency')(value, '$'); }
      else if (filter == "number") { return $filter('number')(value, num); }
      else { return value; }
    }
  };

  $scope.fixDiff = function(value) {
    if (value === '' || value === '0' || value === "0.000" || value === "-0.000") {
      return '--';
    } else {
      return $filter('number')(value, 3);
    }
  };

  var submitClicked = false;
  sendMessage = function() {
    if (submitClicked === true) {
      signalRSvc.sendRequest('RunConfig',$scope.config);
    }
  };

  $scope.reInitialize = function() {
    console.log($scope.hoptService.portNumber);
    signalRSvc.proxy = null;
    signalRSvc.initialize(updateConfigResponses, sendMessage, $scope.hoptService.portNumber, 'ChatHub');
  };

  /*
   * @param {string} method Method Name
   * @param {object} message Data to send
   */
  $scope.sendMessage = function (method, message) {
    if ($scope.configuration.rooms[0].included) { $scope.configuration.rooms[0].num = document.getElementById('room0').value; }
    if ($scope.configuration.rooms[1].included) { $scope.configuration.rooms[1].num = document.getElementById('room1').value; }
    if ($scope.configuration.rooms[2].included) { $scope.configuration.rooms[2].num = document.getElementById('room2').value; }
    if ($scope.configuration.rooms[3].included) { $scope.configuration.rooms[3].num = document.getElementById('room3').value; }
    if ($scope.configuration.rooms[4].included) { $scope.configuration.rooms[4].num = document.getElementById('room4').value; }
    if ($scope.configuration.rooms[5].included) { $scope.configuration.rooms[5].num = document.getElementById('room5').value; }

    if ($scope.configuration.rooms[0].included) { $scope.configuration.rooms[0].originalNum = document.getElementById('originalRoom0').value; }
    if ($scope.configuration.rooms[1].included) { $scope.configuration.rooms[1].originalNum = document.getElementById('originalRoom1').value; }
    if ($scope.configuration.rooms[2].included) { $scope.configuration.rooms[2].originalNum = document.getElementById('originalRoom2').value; }
    if ($scope.configuration.rooms[3].included) { $scope.configuration.rooms[3].originalNum = document.getElementById('originalRoom3').value; }
    if ($scope.configuration.rooms[4].included) { $scope.configuration.rooms[4].originalNum = document.getElementById('originalRoom4').value; }
    if ($scope.configuration.rooms[5].included) { $scope.configuration.rooms[5].originalNum = document.getElementById('originalRoom5').value; }

    $scope.misc.runButton = 'Running...';
    $scope.misc.viewLoading = true;
    submitClicked = true;
    console.log('sending message');
    if (signalRSvc.proxy !== null) {
      console.log('signalRSvc isInitialized');
      signalRSvc.sendRequest(method, message);
    } else {
      console.log('signalRSvc not isInitialized');
      signalRSvc.initialize(updateConfigResponses, sendMessage, $scope.hoptService.portNumber, 'ChatHub');
    }
  };

  signalRSvc.initialize(updateConfigResponses, sendMessage, $scope.hoptService.portNumber, 'ChatHub');
})

;
