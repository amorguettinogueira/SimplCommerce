/*global angular*/
(function () {
    'use strict';

    angular
        .module('simplAdmin.paymentERede', [])
        .config(['$stateProvider',
            function ($stateProvider) {
                $stateProvider
                    .state('payments-erede-config', {
                        url: '/payments/erede/config',
                        templateUrl: '_content/SimplCommerce.Module.PaymentERede/admin/erede/erede-config-form.html',
                        controller: 'ERedeConfigFormCtrl as vm'
                    })
                    ;
            }
        ]);
})();
