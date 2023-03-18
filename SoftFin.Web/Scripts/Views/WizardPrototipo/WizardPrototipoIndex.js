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
    };
});


    

app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {
        //Variaveis
        $scope.locate = "";
        $scope.showManutIntroducao = true;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.modeDetail = false;
        $scope.ListTipo = [{ Value: "G", Text: "Grupo" },{ Value: "P", Text: "Sub-grupo" }, { Value: "S", Text: "Sintética" }, { Value: "A", Text: "Analitica" }];
        $scope.ListNaturezaConta = [{ Value: "AN", Text: "Ativo Normal" }, { Value: "PN", Text: "Passivo Normal" }, { Value: "PL", Text: "Patrimonio Liquido Normal" }, { Value: "RS", Text: "Resultado Normal" }];
        $scope.ListNaturezaContaTodos = [{ Value: "AN", Text: "Ativo Normal" }, { Value: "AR", Text: "Ativo Redutora" }, { Value: "PN", Text: "Passivo Normal" }, { Value: "PR", Text: "Passivo Redutora" }, { Value: "PL", Text: "Patrimonio Liquido Normal" }, { Value: "LR", Text: "Patrimonio Liquido Redutora" }, { Value: "RS", Text: "Resultado Normal" }, { Value: "RR", Text: "Resultado Redutora" }];

        $scope.ListPublicacao = [{ Value: "DC", Text: "Demonstrativos Contabeis (Balanço Patrimonial, DRE, etc)" }, { Value: "BL", Text: "Balancete (detalhado)" }, { Value: "AB", Text: "Ambos" }];
         $scope.ListSubCategoria = [{ Value: "ROB", Text: "Receita Operacional Bruta" }, { Value: "DRB", Text: "Deduções Receita Bruta" }, { Value: "CDO", Text: "Custos da operação/vendas" }, { Value: "DOP", Text: "Despesas da operação" }, { Value: "DFL", Text: "Despesas Financeiras Liquidas" }, { Value: "ORD", Text: "Outras Receitas e despesas não operacionais" }];
       

        //Funcões Staticas
        $scope.MsgError = function (msg, errors) {
            $scope.msgNOK = msg;
            $scope.msgOK = "";
            $scope.errors = errors;
        };


      $scope.IsStep2Valid = function () {
            
            for (var i = 0; i < $scope.lista.length; i++) {
                if ($scope.lista[i].Tipo === 'G') {
                    if ($scope.lista[i].CategoriaGeral === undefined) {
                        $scope.MsgError("Informe todos os campos", null);
                        return false;
                    }
                    if ($scope.lista[i].CategoriaGeral === null) {
                        $scope.MsgError("Informe todos os campos", null);
                        return false;
                    }
                }
            }
            return true;
        };


        $scope.IsStep1Valid = function () {
            
                for (var i = 0; i < $scope.lista.length; i++) {
                    if ($scope.lista[i].Tipo === undefined) {
                        $scope.MsgError("Informe todos os campos", null);
                        return false;
                    }
                    if ($scope.lista[i].Tipo === null) {
                        $scope.MsgError("Informe todos os campos", null);
                        return false;
                    }
             
            }
     
            return true;
        };

        $scope.IsStep5Valid = function () {

            for (var i = 0; i < $scope.lista.length; i++) {
                if ($scope.lista[i].CategoriaGeral === undefined) {
                    $scope.MsgError("Informe todos os campos", null);
                    return false;
                }
                if ($scope.lista[i].CategoriaGeral === null) {
                    $scope.MsgError("Informe todos os campos", null);
                    return false;
                }

            }

            return true;
        };

        $scope.IsStep3Valid = function () {

            for (var i = 0; i < $scope.lista.length; i++) {
                if ($scope.lista[i].IndicacaoPublicacao === undefined) {
                    $scope.MsgError("Informe todos os campos", null);
                    return false;
                }
                if ($scope.lista[i].IndicacaoPublicacao === null) {
                    $scope.MsgError("Informe todos os campos", null);
                    return false;
                }

            }

            return true;
        };

        $scope.IsStep4Valid = function () {
            
            for (var i = 0; i < $scope.lista.length; i++) {
                
                if (($scope.lista[i].Tipo === 'P') || ($scope.lista[i].CategoriaGeral === 'RS')) {
                    if ($scope.lista[i].SubCategoria === undefined) {
                        $scope.MsgError("Informe todos os campos", null);
                        return false;
                    }
                    if ($scope.lista[i].SubCategoria === null) {
                        $scope.MsgError("Informe todos os campos", null);
                        return false;
                    }


                }

                
            }
            return true;
        };


        $scope.setDefaultStep3 = function () {

            for (var i = 0; i < $scope.lista.length; i++) {
                if (($scope.lista[i].Tipo === 'G') ||
                    ($scope.lista[i].Tipo === 'P') ||
                    ($scope.lista[i].Tipo === 'S'))
                    {
                    $scope.lista[i].IndicacaoPublicacao = "AB";
                }
                if ($scope.lista[i].Tipo === 'A') {
                    $scope.lista[i].IndicacaoPublicacao = "BL";
                }
            }
           
        };

       



        $scope.myFilter = function (input) {
            return (input.Tipo === 'G') ? true : false;
            
        };


        $scope.myFilter2 = function (input) {
            return (input.Tipo === 'P') || (input.CategoriaGeral === 'RS')? true : false;

        };

        $scope.MsgSucess = function (msg) {
            $scope.msgOK = msg;
            $scope.msgNOK = "";
        };

        $scope.Order = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };

        
        //Funcões Botôes
        $scope.Search = function () {
            $scope.ObterTodos();
        };

        $scope.ObterTodos = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../WizardPrototipo/ObterTodos/")
                .success(function (data) {
                    $scope.lista = data;
                    Metronic.unblockUI();
                }).error(function (data) {
                    $scope.MsgError("Sistema indisponivel");
                    Metronic.unblockUI();
                });
        }
        $scope.ObterTodos();


        $scope.New = function () {
            $scope.modeDetail = false;
            $scope.ObterPorId(0);
        };

        $scope.ObterPorId = function (value) {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../ProjetoDespesa/ObterPorId", { id: value })
                .success(function (data) {
                    $scope.showGrid = false;
                    $scope.showManut1 = true;
                    Metronic.unblockUI();
                    $scope.entidade = data;
                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.MsgError("Sistema indisponivel");
                });
        };


        $scope.Back = function () {
         if ($scope.showManut1) {
                $scope.showManut1 = false;
                $scope.showManutIntroducao = true;
                $scope.msgNOK = '';
                $scope.msgOK = '';
            }

         else if ($scope.showManut2) {
                $scope.showManut2 = false;
                $scope.showManut1 = true;
                $scope.msgNOK = '';
                $scope.msgOK = '';
            } else if ($scope.showManut3) {
                $scope.showManut3 = false;
                $scope.showManut2 = true;
                $scope.msgNOK = '';
                $scope.msgOK = '';
            } else if ($scope.showManut4) {
                $scope.showManut4 = false;
                $scope.showManut3 = true;
                $scope.msgNOK = '';
                $scope.msgOK = '';
            } else if ($scope.showManut5) {
                $scope.showManut4 = true;
                $scope.showManut5 = false;
                $scope.msgNOK = '';
                $scope.msgOK = '';
            }





        };

        $scope.Edit = function (item) {
            $scope.modeDetail = false;
            $scope.ObterPorId(item.id);
        };
        $scope.Detail = function (item) {
            $scope.modeDetail = true;
            $scope.ObterPorId(item.id);
        };
        $scope.Save = function (item) {
            $scope.Salvar();
        };


        $scope.Next = function () {

            if ($scope.showManutIntroducao) {
                $scope.showManut1 = true;
                $scope.showManutIntroducao = false;
            }


            else if ($scope.showManut1) {
                if ($scope.IsStep1Valid()) {
                    $scope.showManut2 = true;
                    $scope.showManut1 = false;
                    $scope.msgNOK = '';
                    $scope.msgOK = '';
                    parent.scroll(0, 0);
                }
            }
            else if ($scope.showManut2) {
                if ($scope.IsStep2Valid()) {
                    $scope.setDefaultStep3();
                    $scope.showManut3 = true;
                    $scope.showManut2 = false;
                    $scope.msgNOK = '';
                    $scope.msgOK = '';
                    parent.scroll(0, 0);
                }
            } else if ($scope.showManut3) {
                if ($scope.IsStep3Valid()) {
                    $scope.showManut4 = true;
                    $scope.showManut3 = false;
                    $scope.msgNOK = '';
                    $scope.msgOK = '';
                    parent.scroll(0, 0);
                }
            } else if ($scope.showManut4) {
                if ($scope.IsStep4Valid()) {
                    $scope.showManut5 = true;
                    $scope.showManut4 = false;
                    $scope.msgNOK = '';
                    $scope.msgOK = '';
                    parent.scroll(0, 0);
                }
            } else if ($scope.showManut5) {
      
                    parent.scroll(0, 0);
                    /* Função salvar aqui */
                }
            else if ($scope.showManut6){
                $scope.showManut6 = true;
                $scope.showManut5 = false;
            }
            
           
        };


        $scope.Salvar = function () {
            Metronic.blockUI({
                boxed: true
            });
            $http.post("../../ProjetoDespesa/Salvar", $scope.entidade)
                .success(function (data) {
                    Metronic.unblockUI();

                    if (data.CDStatus === "OK") {
                        $scope.MsgSucess(data.DSMessage);
                    } else {
                        $scope.MsgError(data.DSMessage, data.Errors);
                    }
                }).error(function (data) {
                    Metronic.unblockUI();
                    $scope.MsgError("Sistema Indisponivel");
                });
        };

        $scope.Delete = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalExcluir.html',
                controller: ControllerExcluir,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            };

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.Search();
            }, function () {
            });
        };

     
}]);

var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.entidade = item.item;

    $scope.OK = function () {
        $http.post("../../ProjetoDespesa/Excluir", { obj: $scope.entidade })
            .success(function (data) {
                if (data.CDStatus === "OK") {
                    $modalInstance.close(data);
                } else {
                    $scope.showNOK = true;
                    $scope.msgNOK = data.DSMessage;
                    $scope.Erros = data.Erros;
                }

            }).error(function (data) {
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    };

    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    };



};
