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
        return input ===  true ? 'Sim' : 'Não';
    }
})

app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {
        //Variaveis
        $scope.procurar = "";
        $scope.showGrid = true;
        $scope.showManut = false;
        $scope.showOK = false;
        $scope.showNOK = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.msgSalvar = "Salvar";
        $scope.ModoConsulta = false;
        $scope.ListaSIMNAO = [{ Value: true, Text: "Sim" }, { Value: false, Text: "Não" }];
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

            $http.post("../../CI/ObterTodos/")
            .success(function (data) {
                $scope.lista = data;
                Metronic.unblockUI();
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
                Metronic.unblockUI();
            });
        }
        $scope.Novo = function () {
            $scope.ModoConsulta = false;
            $scope.ObterPorId(0);
        }
        $scope.Voltar = function () {
            $scope.showGrid = true;
            $scope.showManut = false;
            $scope.ModoConsulta = false;
            if (!$scope.ObterTodos)
                $scope.Pesquisa();
        }
        $scope.Alterar = function (item) {
            $scope.ModoConsulta = false;
            $scope.ObterPorId(item.id);
        }
        $scope.Detalhar = function (item) {
            $scope.ModoConsulta = true;
            $scope.ObterPorId(item.id);
        }
        $scope.Salvar = function () {
            debugger

            Metronic.blockUI({
                boxed: true
            });

            $scope.msgSalvar = "Aguarde...";
            

            $http.post("../../CI/Salvar", $scope.entidade)
            .success(function (data) {
                Metronic.unblockUI();

                if (data.CDStatus == "OK") {
                    $scope.showOK = true;
                    $scope.msgOK = data.DSMessage;
                    $location.hash('showOKTop');
                    $scope.msgSalvar = "Salvar";

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

                    }, 8000);



            }).error(function (data) {
                Metronic.unblockUI();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";

                $scope.msgSalvar = "Salvar";

            });
        }
        $scope.ObterPorId = function (value) {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../CI/ObterPorId", { id: value } )
            .success(function (data) {
                $scope.showGrid = false;
                $scope.showManut = true;
                Metronic.unblockUI();
                $scope.entidade = data;
            }).error(function (data) {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });
        }
        
        $scope.LoadImposto = function () {
            $http.post("../../CI/ObterImposto")
                .success(function (data) {
                    $scope.ListaImposto = data;
                });
        };
        $scope.CodigoServicoMunicipio = function () {
            $http.post("../../CI/ObterCodigoServicoMunicipio")
                .success(function (data) {
                    $scope.ListaObterCodigoServicoMunicipio = data;
                });
        };
        $scope.ObterTodos();
        $scope.LoadImposto();
        $scope.CodigoServicoMunicipio();


    }]);

