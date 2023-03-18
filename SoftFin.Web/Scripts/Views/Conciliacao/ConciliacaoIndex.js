    $(document).ready(function () {
        Metronic.init(); // init metronic core components
        Layout.init(); // init current layout
        QuickSidebar.init(); // init quick sidebar
    });

    var app = angular.module('SOFTFIN', ['ui.bootstrap', 'ui.utils.masks']);
    //"$scope", "$http", "$location", "ancoraScroll", "$modal"

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
                    moment.locale('pt-BR');
                    ngModelCtrl.$setViewValue(moment(e.localDate).format());
                    scope.$apply();
                    $(this).datetimepicker('hide');
                });
            }
        };
    });

    app.controller('MestreCPAG', [
        '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {

            $scope.SomaOfx = 0;
            $scope.filtro = { data: null, banco_id: 0 };
            $scope.dataCC = '';

            $scope.showPrincipal = false;
            $scope.showAuto = false;
            $scope.showNaoConciliado = false;
            $scope.showConciliado = false;
            $scope.showConciliadoPeriodo = false;



            $scope.LoadListaBanco = function () {
                $http.post("../../Conciliacao/ListaBanco")
                    .success(function (data) {
                        $scope.ListaBanco = data;
                            $scope.filtro.banco_id = data[0].Value;
                    });
            }
            $scope.LoadListaBanco();

            $scope.LoadListaContaContabil = function () {
                $http.post("../../Conciliacao/ListaContaContabil")
                    .success(function (data) {
                        $scope.ListaContaContabil = data;
                    });
            }
            $scope.LoadListaContaContabil();

            $scope.LoadListaTipoDocumento = function () {
                $http.post("../../Conciliacao/ListaTipoDocumento")
                    .success(function (data) {
                        $scope.ListaTipoDocumento = data;
                    });
            }
            $scope.LoadListaTipoDocumento();

            $scope.LoadListaUnidadeNegocio = function () {
                $http.post("../../Conciliacao/ListaUnidadeNegocio")
                    .success(function (data) {
                        $scope.ListaUnidadeNegocio = data;
                    });
            }
            $scope.LoadListaUnidadeNegocio();
            $scope.msgGeral = "";
            $scope.msgBM = "";
            $scope.msgConciliar = "";



            $scope.ExcluirOFX = function (item) {
                $scope.opts = {
                    size: 'lg',
                    animation: true,
                    backdrop: false,
                    backdropClick: false,
                    dialogFade: false,
                    keyboard: true,
                    templateUrl: 'modalExcluirOFX.html',
                    controller: ControllerExcluirOFX,
                    resolve: {}
                };

                $scope.opts.resolve.item = function () {
                    return angular.copy({ item: item });
                }

                var modalInstance = $modal.open($scope.opts);

                modalInstance.result.then(function (value) {
                    item.active = false;
                    $scope.Pesquisar();
                }, function () {
                    item.active = false;
                    $scope.Pesquisar();
                });
            }


            $scope.Pesquisar = function () {
                $scope.OfxMarcados = [];
                $scope.ExtMarcados = [];
                $scope.msgBM = "";
                $scope.msgConciliar = "";
                $scope.SomaOfx = 0;
                $scope.dataCC = "";
                $scope.showPrincipal = true;
                $scope.showAuto = false;
                $scope.showNaoConciliado = false;
                $scope.showConciliado = false;
                $scope.showConciliadoPeriodo = false;
                $scope.PesquisarAuto();
                $scope.PesquisarNaoConciliado();
                $scope.PesquisarConciliado();
                $scope.PesquisarConciliadoPeriodo();
            }

            $scope.Pesquisar2 = function () {
                $scope.OfxMarcados = [];
                $scope.ExtMarcados = [];
                $scope.msgBM = "";
                $scope.msgConciliar = "";
                $scope.SomaOfx = 0;
                $scope.dataCC = "";
                $scope.showPrincipal = true;
                $scope.showAuto = false;
                $scope.showNaoConciliado = false;
                $scope.showConciliadoPeriodo = false;
                $scope.PesquisarAuto();
                $scope.PesquisarNaoConciliado();
                $scope.PesquisarConciliadoPeriodo();
            }

            $scope.PesquisarAuto = function () {
                $scope.ListaConciliarAuto = null;
                $scope.showAuto = false;
                $http.post("../../Conciliacao/CarregaAutoConciliados", { data: $scope.filtro.data, banco_id: $scope.filtro.banco_id })
                    .success(function (data) {
                        $scope.ListaConciliarAuto = data;
                        $scope.showAuto = true;
                    })
                    .error(function () {
                        $scope.showPrincipal = false;
                        $scope.msgGeral = "Erro ao pesquisar verifique os parametros.";
                    });
            };

            $scope.PesquisarNaoConciliado = function () {
                $http.post("../../Conciliacao/CarregaNaoConciliado", { data: $scope.filtro.data, banco_id: $scope.filtro.banco_id })
                    .success(function (data) {
                        $scope.ListaLanctoExtratos = data.LanctoExtratos;
                        $scope.PesquisarBM(1);
                        
                    })
                    .error(function () {
                        $scope.showPrincipal = false;
                        $scope.msgGeral = "Erro ao pesquisar verifique os parametros.";
                    });
            };

            $scope.PesquisarConciliado = function () {
                $http.post("../../Conciliacao/ConsultaConciliados", { data: $scope.filtro.data, banco_id: $scope.filtro.banco_id })
                    .success(function (data) {
                        $scope.ListaConciliados = data;
                        $scope.showConciliado = true;
                    })
                    .error(function () {
                        $scope.showPrincipal = false;
                        $scope.msgGeral = "Erro ao pesquisar verifique os parametros.";
                    });
            };

            $scope.PesquisarConciliadoPeriodo = function () {
                $http.post("../../Conciliacao/ConsultaConciliadosPeriodo", { data: $scope.filtro.data, banco_id: $scope.filtro.banco_id })
                    .success(function (data) {
                        $scope.ListaConsultaConciliados = data;
                        $scope.showConciliadoPeriodo = true;
                    })
                    .error(function () {
                        $scope.showPrincipal = false;
                        $scope.msgGeral = "Erro ao pesquisar verifique os parametros.";
                    });
            };

            $scope.PesquisarBM = function (value) {

                if ($scope.dataCC == "")
                {
                    var jsonaux = { data: $scope.filtro.data, banco_id: $scope.filtro.banco_id, tipo: value, valor: $scope.SomaOfx };
                }
                else {
                    var jsonaux = { data: $scope.dataCC, banco_id: $scope.filtro.banco_id, tipo: value, valor: $scope.SomaOfx };
                }

                $http.post("../../Conciliacao/PesquisarBMAjs", jsonaux)
                    .success(function (data) {
                        $scope.ListaBancoMovimentoConciliacaoVLWs = data.BancoMovimentoConciliacaoVLWs;
                        $scope.dataCC = data.data;
                        $scope.showNaoConciliado = true;
                    })
                    .error(function () {
                        $scope.showPrincipal = false;
                        $scope.msgGeral = "Sistema Indisponivel";
                    });
            }

            $scope.Excluir = function (item) {
                $scope.showConciliado = false;
                $http.post("../../Conciliacao/ExcluirAjs", { id: item.id })
                    .success(function (data) {
                        var index = $scope.ListaConciliados.indexOf(item);
                        $scope.ListaConciliados.splice(index, 1);

                        $scope.showConciliado = true;
                        $scope.Pesquisar2();
                    })
                    .error(function () {
                        $scope.msgGeral = "Sistema Indisponivel";
                    });
            }


            $scope.ConciliarAuto = function (item) {
                $scope.showAuto = false;
                $http.post("../../Conciliacao/ConciliarAutoAjs", item)
                    .success(function (data) {
                        $scope.showAuto = true;
                        $scope.Pesquisar();
                    })
                    .error(function () {
                        $scope.msgGeral = "Sistema Indisponivel";
                    });
            }

            $scope.Incluirbm = function (item) {
                $scope.showPrincipal = false;
                $http.post("../../Conciliacao/IncluirBMAjs", item)
                    .success(function (data) {
                        
                        $scope.Pesquisar();
                    })
                    .error(function () {
                        $scope.msgGeral = "Sistema Indisponivel";
                    });
            }

            $scope.SalvaBM = function () {
                $scope.showPrincipal = false;
                $http.post("../../Conciliacao/SalvaBMAjs",
                    {
                        planoDeConta_id: $scope.BM.planoDeConta_id,
                        tipoDeDocumento_id: $scope.BM.tipoDeDocumento_id,
                        unidadeDeNegocio_id: $scope.BM.unidadeDeNegocio_id,
                        historico: $scope.BM.historico,
                        Ofxs: $scope.OfxMarcados
                    })
                    .success(function (data) {
                        $scope.msgBM = "Salvo e conciliado com sucesso";
                        $scope.Pesquisar();
                    })
                    .error(function () {
                        $scope.msgGeral = "Sistema Indisponivel";
                    });
            }

            $scope.Conciliar = function () {
                
                $scope.showNaoConciliado = false;
                $http.post("../../Conciliacao/ConciliarAjs",
                    {
                        Ofxs: $scope.OfxMarcados,
                        Exts: $scope.ExtMarcados,
                        idBanco: $scope.filtro.banco_id,
                        dataConciliacao: $scope.filtro.data
                    })
                    .success(function (data) {
                        $scope.showNaoConciliado = true;
                        $scope.msgConciliar = "Salvo e conciliado com sucesso";
                        $scope.Pesquisar();
                    })
                    .error(function () {
                        $scope.msgGeral = "Sistema Indisponivel";
                    });
            }

            $scope.MarcaOFX = function (item) {
                item.selecionado = !item.selecionado;
                for (var i = 0; i < $scope.OfxMarcados.length; i++) {
                    if ($scope.OfxMarcados[i] == item.id) {
                        $scope.OfxMarcados.splice(i);
                        $scope.SomaOfx -= item.Valor;
                        return
                    }
                }
                $scope.OfxMarcados.push(item.id);
                $scope.SomaOfx += item.Valor;
            }

            $scope.Marcachk = function (item) {
                item.selecionado = !item.selecionado;
            }

            $scope.ConciliaTodosAuto = function () {
                $scope.opts = {
                    size: 'lg',
                    animation: true,
                    backdrop: false,
                    backdropClick: false,
                    dialogFade: false,
                    keyboard: true,
                    templateUrl: 'modalConciliarSelecionadosAuto.html',
                    controller: ControllerConciliarAutoListaOFX,
                    resolve: {}
                };

                $scope.opts.resolve.item = function () {
                    return angular.copy({ item: $scope.ListaConciliarAuto });
                }

                var modalInstance = $modal.open($scope.opts);

                modalInstance.result.then(function (value) {

                    $scope.Pesquisar();
                }, function () {

                });
            }

            $scope.ExcluirTodosOFX = function () {
                $scope.opts = {
                    size: 'lg',
                    animation: true,
                    backdrop: false,
                    backdropClick: false,
                    dialogFade: false,
                    keyboard: true,
                    templateUrl: 'modalExcluirSelecionadosOFX.html',
                    controller: ControllerExcluirOFXLista,
                    resolve: {}
                };

                $scope.OFXListaMarcados = [];
                //debugger
                for (var i = 0; i < $scope.ListaLanctoExtratos.length; i++) {
                    if ($scope.ListaLanctoExtratos[i].selecionado) {
                        $scope.OFXListaMarcados.push($scope.ListaLanctoExtratos[i].id);

                    }
                }

                $scope.opts.resolve.item = function () {
                    return angular.copy({ item: $scope.OFXListaMarcados });
                }

                var modalInstance = $modal.open($scope.opts);

                modalInstance.result.then(function (value) {

                    $scope.Pesquisar();
                }, function () {

                });
            }
            $scope.MarcaExt = function (item) {
                for (var i = 0; i < $scope.ExtMarcados.length; i++) {
                    if ($scope.ExtMarcados[i] == item.id) {
                        $scope.ExtMarcados.splice(i);

                        return
                    }
                }
                $scope.ExtMarcados.push(item.id);
            }

            $scope.MarcaTodosAuto = function () {
                $scope.valorTotal = 0;
                
                if ($scope.ListaConciliarAuto[0].selecionado == undefined)
                    var selecionado = true;
                else
                    var selecionado = $scope.ListaConciliarAuto[0].selecionado;

                for (var i = 0; i < $scope.ListaConciliarAuto.length ; i++) {
                    $scope.ListaConciliarAuto[i].selecionado = !selecionado;
                }

            }

            $scope.MarcaTodosOFX = function () {
                //debugger
                $scope.valorTotal = 0;

                if ($scope.ListaLanctoExtratos[0].selecionado == undefined)
                    var selecionado = false;
                else
                    var selecionado = $scope.ListaLanctoExtratos[0].selecionado;

                for (var i = 0; i < $scope.ListaLanctoExtratos.length; i++) {
                    $scope.ListaLanctoExtratos[i].selecionado = !selecionado;
                }


            }

            $scope.UploadOFX = function () {
                $scope.opts = {
                    size: 'lg',
                    animation: true,
                    backdrop: false,
                    backdropClick: false,
                    dialogFade: false,
                    keyboard: true,
                    templateUrl: 'modalUpload.html',
                    controller: ControllerUpload,
                    resolve: {} // empty storage
                };

                $scope.opts.resolve.item = function () {
                    return angular.copy({ item: 0 }); // pass name to Dialog
                }

                var modalInstance = $modal.open($scope.opts);

                modalInstance.result.then(function (value) {
                    //console.log("Modal OK");
                }, function () {
                    //on cancel button press
                    //console.log("Modal Closed");
                });
            };



        }
    ]);


    var ControllerExcluirOFX = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

        $scope.item = item.item;
        $scope.someMsg = function () {
            $scope.showNOK = false;
            $scope.showOK = false;
        };



        $scope.ok = function () {
            $http.post("../../../Conciliacao/ExcluirOFX/" + $scope.item.id).success(function (data) {
	            if (data.CDStatus == "OK") {
	                $scope.showOK = true;
	                $scope.msgOK = data.DSMessage;
	            } else {
	                $scope.showNOK = true;
	                $scope.msgNOK = data.DSMessage;
	            };

	            $timeout(
                    function () {
                        $scope.showNOK = false;
                        $scope.showOK = false;

                    }, 8000);

	            $modalInstance.close();
	        }).error(function (data) {
	            $scope.showNOK = true;
	            $scope.msgNOK = "Sistema indisponivel";
	        });
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }

    var ControllerConciliarAutoListaOFX = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

        $scope.item = item.item;
        $scope.someMsg = function () {
            $scope.showNOK = false;
            $scope.showOK = false;
        };

        $scope.emConsulta = false;

        $scope.ok = function () {
            $scope.emConsulta = true;
            $http.post("../../../Conciliacao/ConciliarAutoAjsLista", $scope.item).success(function (data) {
                $scope.emConsulta = false;
                if (data.CDStatus == "OK") {
                    $scope.showOK = true;
                    $scope.msgOK = data.DSMessage;
                } else {
                    $scope.showNOK = true;
                    $scope.msgNOK = data.DSMessage;
                };

                $timeout(
                    function () {
                        $scope.showNOK = false;
                        $scope.showOK = false;

                    }, 8000);

                $modalInstance.close();
            }).error(function (data) {
                $scope.emConsulta = false;
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }




    var ControllerUpload = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

        
        $scope.arquivo = "";

        $scope.someMsg = function () {
            $scope.showNOK = false;
            $scope.showOK = false;
        };

        $scope.ok = function () {
            
            var f = document.getElementById('newPhotos').files[0];
            var fd = new FormData();
            fd.append("file", f);
            
            
            $http.post("../../../Conciliacao/UploadOFX", fd, {
                withCredentials: true,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            }).success(function (data) {
                if (data.CDStatus == "OK") {
                    $scope.showOK = true;
                    $scope.msgOK = data.DSMessage;
                } else {
                    $scope.showNOK = true;
                    $scope.msgNOK = data.DSMessage;
                };

                //$timeout(
                //    function () {
                //        $scope.showNOK = false;
                //        $scope.showOK = false;

                //    }, 8000);

                //$modalInstance.close(value);
            }).error(function (data) {
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }


var ControllerExcluirOFXLista = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.item = item.item;
    $scope.someMsg = function () {
        $scope.showNOK = false;
        $scope.showOK = false;
    };
    
    $scope.ok = function () {
        $http.post("../../../Conciliacao/ExcluirOFXLista", $scope.item).success(function (data) {
            if (data.CDStatus == "OK") {
                $scope.showOK = true;
                $scope.msgOK = data.DSMessage;
            } else {
                $scope.showNOK = true;
                $scope.msgNOK = data.DSMessage;
            };

            $timeout(
                function () {
                    $scope.showNOK = false;
                    $scope.showOK = false;

                }, 8000);

            $modalInstance.close();
        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}
