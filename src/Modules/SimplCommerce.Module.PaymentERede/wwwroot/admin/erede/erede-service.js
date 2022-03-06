/*global angular*/
(function () {
    angular
        .module('simplAdmin.paymentERede')
        .factory('paymentERedeService', ['$http', paymentERedeService]);

    function paymentERedeService($http) {
        var service = {
            getSettings: getSettings,
            updateSetting: updateSetting
        };
        return service;

        function getSettings() {
            return $http.get('api/erede/config');
        }

        function updateSetting(settings) {
            return $http.put('api/erede/config', settings);
        }
    }
})();
