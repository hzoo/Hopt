var ngBoilerplate = angular.module( 'ngBoilerplate', [
  'templates-app',
  'templates-common',
  'ngBoilerplate.home',
  'ngBoilerplate.inputs',
  'ngBoilerplate.config',
  'ngBoilerplate.opt',
  'ui.router',
  'ngAnimate'
])

.config( function myAppConfig ( $stateProvider, $urlRouterProvider ) {
  $urlRouterProvider.otherwise( '/home' );
})

.run( function run ($rootScope, $state, $stateParams) {
  $rootScope.$state = $state;
 $rootScope.$stateParams = $stateParams;
})

.controller( 'AppCtrl', function AppCtrl ( $scope, $location ) {
  $scope.name = "Hopt";
  $scope.stateIsHome = false;
  $scope.$on('$stateChangeSuccess', function(event, toState, toParams, fromState, fromParams){
    if ( angular.isDefined( toState.data.pageTitle ) ) {
      $scope.pageTitle = toState.data.pageTitle + ' | ' + $scope.name ;
    }
     if (toState.name === "home") {
      $scope.stateIsHome = true;
     } else {
      $scope.stateIsHome = false;
     }
  });
  //angular ui navbar collapse
  // $scope.isCollapsed = true;
  // $scope.$watch('isCollapsed', function(value) {
  //   console.log(value);
  // });
})

.directive('scrollSpy', function($window, $state) {
  return {
    restrict: 'A',
    controller: function($scope) {
      $scope.spies = [];
      this.addSpy = function(spyObj) {
        return $scope.spies.push(spyObj);
      };
    },
    link: function(scope, elem, attrs) {
      var spyElems = [];
      scope.$watch('spies', function(spies) {
        for (var i = 0; i < spies.length; i++) {
          if (spyElems[spies[i].id] === undefined) {
            spyElems[spies[i].id] = angular.element('#' + spies[i].id);
          }
        }
      });
      return $($window).scroll(function() {
        if ($state.includes("inputs")) {
          var highlightSpy, pos, spy, _ref;
          highlightSpy = null;
          _ref = scope.spies;
          for (var i = 0; i < scope.spies.length; i++) {
            spy = _ref[i];
            spy.out();
            if ((pos = document.getElementById(spy.id).offsetTop) - $window.scrollY < 0) {
            // if ((pos = spyElems[spy.id].offset().top) - $window.scrollY <= 0) {
              spy.pos = pos;
              if (highlightSpy === null) {
                highlightSpy = spy;
              }
              if (highlightSpy.pos < spy.pos) {
                highlightSpy = spy;
              }
            }
          }
          return highlightSpy !== null ? highlightSpy["in"]() : void 0;
        }
      });
    }
  };
})


// .directive('spy', function() {
//   return {
//     restrict: "A",
//     require: "^scrollSpy",
//     link: function(scope, elem, attrs, affix) {
//       return affix.addSpy({
//         id: attrs.spy,
//         "in": function() {
//           return elem.addClass('active');
//         },
//         out: function() {
//           return elem.removeClass('active');
//         }
//       });
//     }
//   };
// })

.directive("scrollTo", ["$window", function($window) {
  return {
    restrict : "AC",
    compile : function() {
        function scrollInto(elementId) {
            if(!elementId) {
              $window.scrollTo(0, 0);
            }
            //check if an element can be found with id attribute
            var el = document.getElementById(elementId);
            if(el) {
              //el.scrollIntoView();
              // $window.scrollTo(0, el.offsetTop);
              $("body").animate({scrollTop: el.offsetTop}, 400);
            }
        }

        return function(scope, element, attr) {
            element.bind("click", function(event){
                scrollInto(attr.scrollTo);
            });
        };
    }
  };
}])

;