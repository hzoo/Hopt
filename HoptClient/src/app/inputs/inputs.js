angular.module( 'ngBoilerplate.inputs', [
  'ui.router',
  'ngTable'
])

.config(function config( $stateProvider) {
  $stateProvider.state( 'inputs', {
    url: '/inputs',
    views: {
      "main": {
        controller: 'InputsCtrl',
        templateUrl: 'inputs/inputs.tpl.html'
      }
    },
    data:{ pageTitle: 'Inputs' }
  });
})

.controller( 'InputsCtrl', function InputsCtrl( $scope, $location, $anchorScroll, Configuration, HospitalData, ngTableParams ) {

  //just for categories
  var sidebarNames = [
  "Client Information",
  "Arrivals",
  "Acuity Breakdown",
  "Service Times",
  "Constraints",
  "Costs",
  "Simulation (Advanced)"
  ];
  var sidebarIds = [
  "client-info",
  "arrivals",
  "acuity-breakdown",
  "service-times",
  "constraints",
  "costs",
  "simulation"
  ];

  $scope.page = {};
  $scope.page.sidebarInputs = [];
  for (i = 0; i < sidebarNames.length; i++) {
    $scope.page.sidebarInputs.push({name: sidebarNames[i],id: sidebarIds[i]});
  }

  //for sidebar
  $scope.selected = null;
  $scope.select= function(item) {
     $scope.selected = item;
  };
  $scope.isActive = function(item) {
     return $scope.selected === item;
  };

  //service times
  $scope.changeServiceTimes = function(string) {
    if (string === "exp") {
      angular.forEach($scope.hospitalData.serviceInfo, function(type) {
        type.averageRoomTime = 'Random.Exponential(0.1)';
      });
    } else if (string === "tri") {
      angular.forEach($scope.hospitalData.serviceInfo, function(type) {
        type.averageRoomTime = 'Random.Triangular(1,' + type.averageRoomTime + ',3)';
      });
    } else if (string === "uni") {
      angular.forEach($scope.hospitalData.serviceInfo, function(type) {
        if (('' + type.averageRoomTime).indexOf(',') !== -1) {
          type.averageRoomTime = type.averageRoomTime.split(',')[1];
        }
      });
    }
  };

  //data
  // $scope.hospitalData = HospitalData.getHospitalData();
  $scope.hospitalData = HospitalData.hospitalData;
  $scope.configuration = Configuration.getConfiguration();
})

//http://jsfiddle.net/odiseo/dj6mX/
// .directive('currencyInput', function currencyInput() {
//   return {
//       restrict: 'A',
//       scope: {
//           field: '=',
//           dec: '@'
//       },
//       replace: true,
//       template: '<input type="text" ng-model="field"></input>',
//       link: function(scope, element, attrs) {

//           $(element).bind('keyup', function(e) {
//               var input = element.find('input');
//               var inputVal = input.val();

//               //clearing left side zeros
//               while (scope.field.charAt(0) == '0') {
//                   scope.field = scope.field.substr(1);
//               }

//               scope.field = scope.field.replace(/[^\d.\',']/g, '');

//               var point = scope.field.indexOf(".");
//               if (point >= 0) {
//                   if (scope.dec > 0) {
//                     scope.field = scope.field.slice(0, point + parseInt(scope.dec,10) + 1);
//                   } else {
//                     scope.field = scope.field.slice(0, point);
//                   }
//               }

//               var decimalSplit = scope.field.split(".");
//               var intPart = decimalSplit[0];
//               var decPart = decimalSplit[1];

//               intPart = intPart.replace(/[^\d]/g, '');
//               if (intPart.length > 3) {
//                   var intDiv = Math.floor(intPart.length / 3);
//                   while (intDiv > 0) {
//                       var lastComma = intPart.indexOf(",");
//                       if (lastComma < 0) {
//                           lastComma = intPart.length;
//                       }

//                       if (lastComma - 3 > 0) {
//                           intPart = intPart.splice(lastComma - 3, 0, ",");
//                       }
//                       intDiv--;
//                   }
//               }

//               if (decPart === undefined) {
//                   decPart = "";
//               }
//               else {
//                   decPart = "." + decPart;
//               }
//               var res = intPart + decPart;

//               scope.$apply(function() {scope.field = res;});
//           });
//       }
//   };
// })

;

// String.prototype.splice = function(idx, rem, s) {
//     return (this.slice(0, idx) + s + this.slice(idx + Math.abs(rem)));
// };