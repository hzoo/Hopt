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
    data:{ pageTitle: 'Search' }
  });
})

.factory('signalRSvc2', function signalRSvc($, $rootScope) {
    return {
        proxy: null,
        initialize: function (responseCallback, connectionCallback, port, hub) {
            if (this.proxy === null) {
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
              this.proxy.on('getResponses', function (message,message2) {
                // console.log(message);
                // console.log(responseCallback);
                  $rootScope.$apply(function () {
                      responseCallback(message,message2);
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

  function calculateMaxRoomConfig() {
    for (var i = 0; i < $scope.configuration.rooms.length; i++) {
      var annualVisits = $scope.hospitalData.arrivalInfo[0].value;
      var annualVisitsPerRoom = 0;
       if ($scope.configuration.rooms[1].included === false && $scope.configuration.rooms[2].included === true) {
        if (i === 0) {
          annualVisitsPerRoom = annualVisits * ($scope.hospitalData.acuityInfo[0].value + $scope.hospitalData.acuityInfo[1].value + $scope.hospitalData.acuityInfo[2].value * 0.5) / 100;
        } else if (i === 1) {
          annualVisitsPerRoom = 0;
        } else if (i === 2) {
          annualVisitsPerRoom = annualVisits * ($scope.hospitalData.acuityInfo[2].value * 0.5 + $scope.hospitalData.acuityInfo[3].value + $scope.hospitalData.acuityInfo[4].value) / 100;
        }
      }
      else if ($scope.configuration.rooms[1].included === true && $scope.configuration.rooms[2].included === false) {
        if (i === 0) {
          annualVisitsPerRoom = annualVisits * ($scope.hospitalData.acuityInfo[1].value + $scope.hospitalData.acuityInfo[2].value + $scope.hospitalData.acuityInfo[3].value + $scope.hospitalData.acuityInfo[4].value) / 100;
        } else if (i === 1) {
          annualVisitsPerRoom = annualVisits * $scope.hospitalData.acuityInfo[0].value / 100;
        } else if (i === 2) {
          annualVisitsPerRoom = 0;
        }
      } else if ($scope.configuration.rooms[1].included === false && $scope.configuration.rooms[2].included === false) {
        if (i === 0) {
          annualVisitsPerRoom = annualVisits;
        } else if (i === 1) {
          annualVisitsPerRoom = 0;
        } else if (i === 2) {
          annualVisitsPerRoom = 0;
        }
      } else {
        if (i === 0) {
          annualVisitsPerRoom = annualVisits * ($scope.hospitalData.acuityInfo[1].value + $scope.hospitalData.acuityInfo[2].value * 0.5) / 100;
        } else if (i === 1) {
          annualVisitsPerRoom = annualVisits * $scope.hospitalData.acuityInfo[0].value / 100;
        } else if (i === 2) {
          annualVisitsPerRoom = annualVisits * ($scope.hospitalData.acuityInfo[2].value * 0.5 + $scope.hospitalData.acuityInfo[3].value + $scope.hospitalData.acuityInfo[4].value) / 100;
        }
      }

      if (i === 3) {
        annualVisitsPerRoom =  annualVisits * ($scope.hospitalData.acuityInfo[0].value * 0.45 + $scope.hospitalData.acuityInfo[1].value * 0.25 + $scope.hospitalData.acuityInfo[2].value * 0.15 + $scope.hospitalData.acuityInfo[3].value * 0.1 + $scope.hospitalData.acuityInfo[4].value * 0.1) / 100;
      } else if (i === 4) {
        annualVisitsPerRoom =  annualVisits * ($scope.hospitalData.acuityInfo[1].value * 0.1 + $scope.hospitalData.acuityInfo[2].value*0.05) / 100;
      } else if (i === 5) {
        annualVisitsPerRoom =  annualVisits * ($scope.hospitalData.acuityInfo[0].value * 0.45 + $scope.hospitalData.acuityInfo[1].value * 0.25 + $scope.hospitalData.acuityInfo[2].value * 0.15) / 100;
      }

      console.log(annualVisits, i,annualVisitsPerRoom);

      var peakMonth = annualVisitsPerRoom * 0.1;
      var avgDay = peakMonth / 30.5;
      var peakDay = avgDay + (2.33*Math.sqrt(avgDay));
      var peakShift;
      if (i == 2) {
        peakShift= peakDay;
      } else {
        peakShift = peakDay * 0.5;
      }

      var avgRoomTime;
      if ($scope.hospitalData.serviceInfo[i].averageRoomTime.indexOf("Exponential") != -1)
      {
         avgRoomTime = $scope.hospitalData.serviceInfo[i].averageRoomTime.substring(19,$scope.hospitalData.serviceInfo[i].averageRoomTime.length-1);
      }
      else
      {
        avgRoomTime = $scope.hospitalData.serviceInfo[i].averageRoomTime;
      }
	console.log(avgRoomTime);
      var proceduresPerShiftPerRoom;
      if (i === 2) {
        proceduresPerShiftPerRoom= 14.0 / Number(avgRoomTime);
      } else if (i === 0 || i === 1) {
        proceduresPerShiftPerRoom = 8.0 / Number(avgRoomTime);
      }  else if (i === 3 || i === 4 || i === 5) {
        proceduresPerShiftPerRoom = 24.0 / Number(avgRoomTime);
      }
      var numRooms;
      if (i === 3 || i === 4 || i === 5) {
        numRooms= peakDay / proceduresPerShiftPerRoom;
      } else {
        numRooms = peakShift / proceduresPerShiftPerRoom;
      }
      $scope.configuration.rooms[i].maxNum = Math.ceil(numRooms);//+1; //increase max
      if ($scope.configuration.rooms[i].maxNum < $scope.configuration.rooms[i].originalNum) {
        $scope.configuration.rooms[i].maxNum = $scope.configuration.rooms[i].originalNum;
      } else {
        // console.log(annualVisits);
        // console.log(annualVisitsPerRoom);
        // console.log(peakMonth);
        // console.log(avgDay);
        // console.log(peakDay);
        // console.log(peakShift);
        // console.log(proceduresPerShiftPerRoom);
        // console.log(numRooms);
        // console.log($scope.configuration.rooms[i].maxNum);
      }
    }
  }
  calculateMaxRoomConfig();

  $scope.gridOptions = {
    data: 'hoptService.responses',
    showGroupPanel: false,
    showFilter: true,
    showColumnMenu: true,
    enableColumnResize: true
  };

  $scope.misc.responses = $scope.hoptService.lastResponses;
  $scope.misc.optResponses = $scope.hoptService.optResponses;
  $scope.optFinished = false;

  updateConfigResponses = function (response,response2) {
    $scope.misc.viewLoading = false;

    var configuration = response;
    var configResponse = response2;

    for (var i = 0; i < $scope.configuration.rooms.length; i++) {
      $scope.configuration.rooms[i].optNum = configuration.rooms[i].num;
    }

    angular.forEach(configResponse, function(value, key) {
      for (var i = 0; i < $scope.misc.optResponses.length; i++) {
        if ($scope.misc.optResponses[i].name == key) {
          $scope.misc.optResponses[i].value = value;
        }
      }
    });

    $scope.hoptService.initialCost = configResponse.initialCost;
    $scope.hoptService.annualCost = configResponse.annualCost;
    $scope.hoptService.totalCost = configResponse.totalCost;
    $scope.optFinished = true;
    $scope.misc.runTime = (new Date().getTime() - $scope.misc.runTime) / 1000 + " sec";
  };

  $scope.stopOpt = function() {
    $scope.runOpt = false;
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
      return $filter('number')(value, 4);
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
    $scope.misc.runTime = new Date().getTime();

    // roomIterator = [0,0,0,0,0,0];
    $scope.runOpt = true;
    $scope.optFinished = false;

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