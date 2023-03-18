$(document).ready(function () {
    Metronic.init(); // init metronic core components
    Layout.init(); // init current layout
    QuickSidebar.init(); // init quick sidebar
});

var app = angular.module('SOFTFIN', ['ui.bootstrap', 'ui.utils.masks', 'angularUtils.directives.dirPagination']);
app.directive('datetimez', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {

            element.datetimepicker({
                dateFormat: 'dd/MM/yyyy',
                language: 'ptbr',
                pickTime: false,
                startDate: '01-11-2013',      // set a minimum date
                endDate: '01-11-2030'          // set a maximum date
            }).on('changeDate', function (e) {
                ngModelCtrl.$setViewValue(moment(e.localDate).format());
                scope.$apply();
                $(this).datetimepicker('hide');
            });
        }
    };
});

app.filter('yesNo', function () {
    return function (input) {
        return input === true ? 'Sim' : 'Não';
    }
})

app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {
        //Variaveis
        $scope.procurar = "";
        $scope.showManut = false;
        $scope.ModoAlterar = false;
        $scope.showOK = false;
        $scope.showNOK = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.msgSalvar = "Salvar";
        //Funcões Staticas
        $scope.MsgError = function (msg) {
            $scope.msgNOK = msg;
            $scope.showNOK = true;
            $scope.showOK = false;
        }
        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };

        //Funcões Botôes
        $scope.ObterTodos = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../CCvsCD/ObterTodos/")
                .success(function (data) {
                    $scope.lista = data;
                    Metronic.unblockUI();
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                    Metronic.unblockUI();
                });
        }
        $scope.Voltar = function () {
            $scope.ModoAlterar = false;
            $scope.ModoConsulta = false;
            $scope.ObterTodos();
        }
        
        //$scope.CarregaDS = function (item) {
        //    item.cdds = "";
        //    angular.forEach(ListaPlanoContas, function (value, key) {
                
        //        if (key === item.planoDeContas_id) {
        //            item.cdds = value;
        //        }
                
        //    });
        //}


        $scope.Alterar = function (item) {
            $scope.ModoAlterar = true;
            $scope.showOK = false;
            $scope.showNOK = false;
        }
        $scope.Salvar = function () {


            Metronic.blockUI({
                boxed: true
            });

            $scope.msgSalvar = "Aguarde...";

            $http.post("../../CCvsCD/Salvar", $scope.lista)
                .success(function (data) {
                    Metronic.unblockUI();

                    if (data.CDStatus === "OK") {
                        $scope.showOK = true;
                        $scope.msgOK = data.DSMessage;
                        $scope.msgSalvar = "Salvar";
                        $scope.Voltar();

                    } else {
                        $scope.showNOK = true;
                        $scope.Erros = data.Erros;
                        $scope.msgNOK = data.DSMessage;
                        $scope.msgSalvar = "Salvar";
                    }

                    $timeout(
                        function () {
                            $scope.showNOK = false;
                            $scope.showOK = false;
                            $scope.msgSalvar = "Salvar";

                        }, 18000);



                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.showNOK = true;
                    $scope.msgNOK = "Sistema indisponivel";
                    $scope.msgSalvar = "Salvar";
                });
        }




        $scope.LoadObterPlanoContas = function () {
            $http.post("../../CG/ObterPlanoContasDebito/")
                .success(function (data) {
                    $scope.ListaPlanoContas = data;
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                });
        }
        $scope.LoadObterPlanoContas();
        $scope.ObterTodos();
    }]);





