/*global angular, jQuery*/
(function ($) {
    angular
        .module('simplAdmin.paymentERede')
        .controller('ERedeConfigFormCtrl', ['paymentERedeService', 'translateService', ERedeConfigFormCtrl]);

    function ERedeConfigFormCtrl(paymentERedeService, translateService) {
        var vm = this;
        vm.translate = translateService;
        vm.eredeConfig = {};

        vm.save = function save() {
            vm.validationErrors = [];
            paymentERedeService.updateSetting(vm.eredeConfig)
                .then(function (result) {
                    toastr.success('Application settings have been saved');
                })
                .catch(function (response) {
                    var error = response.data;
                    vm.validationErrors = [];
                    if (error && angular.isObject(error)) {
                        for (var key in error) {
                            vm.validationErrors.push(error[key][0]);
                        }
                    } else {
                        vm.validationErrors.push('Não foi possível salvar as configurações da eRede.');
                    }
                });
        };

        function init() {
            paymentERedeService.getSettings().then(function (result) {
                vm.eredeConfig = result.data;
            });
        }

        init();
    }
})(jQuery);
