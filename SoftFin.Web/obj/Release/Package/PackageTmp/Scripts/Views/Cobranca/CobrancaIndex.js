$(document).ready(function () {
    Metronic.init(); // init metronic core components
    Layout.init(); // init current layout
    QuickSidebar.init(); // init quick sidebar
});

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return '';
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

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
        return input === "True" ? 'Sim' : 'Não';
    };
});





app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {
        $scope.Lista = [];
        $scope.reverse = false;
        $scope.procurar = "";


        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];

        $scope.Pesquisar = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../Cobranca/ObterCobranca/")
                .success(function (data) {
                    $scope.Lista = data;
                    Metronic.unblockUI();
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                    Metronic.unblockUI();
                });
        };
        $scope.Pesquisar();

        $scope.Enviar = function () {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalEmail.html',
                controller: ControllerEmail,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: $scope.Lista }); // pass name to Dialog
            };

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.LoadBM();
            }, function () {
            });
        };


    }
]);

var ControllerEmail = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.Lista = item.item;
    $scope.showNOK = false;
    $scope.showOK = false;
    $scope.msgNOK = "";
    $scope.Titulo = "";
    $scope.Texto = "";

    $scope.OK = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../Cobranca/Salvar", { titulo: $scope.Titulo, texto: $scope.Texto, objs: $scope.Lista })
            .success(function (data) {
                Metronic.unblockUI();
            if (data.CDStatus === "OK") {
                $modalInstance.close(data);
            } else {
                $scope.showNOK = true;
                $scope.msgNOK = data.DSMessage;
            }
            $timeout(
                function () {
                    $scope.showNOK = false;
                    $scope.showOK = false;
                }, 8000);
            }).error(function (data) {
                Metronic.unblockUI();
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}



var ControllerSalvar = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.entidade = item.item;
    $scope.mensagem = "";

    Metronic.blockUI({
        boxed: true
    });

    $http.post("../../BM/verificaDuplicidade", $scope.entidade)
        .success(function (data) {
            Metronic.unblockUI();
            if (data.CDStatus == "NOK") {
                $scope.mensagem = data.DSMessage;
            } 
        }).error(function (data) {
            Metronic.unblockUI();
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });


    $scope.ok = function () {
        $http.post("../../BM/Salvar", $scope.entidade )
        .success(function (data) {
            if (data.CDStatus == "OK") {
                $modalInstance.close(data);
            } else {
                $scope.showNOK = true;
                $scope.msgNOK = data.DSMessage;
                $scope.Erros = data.Erros;
            }
            $timeout(
                function () {
                    $scope.showNOK = false;
                    $scope.showOK = false;

                }, 8000);
        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}