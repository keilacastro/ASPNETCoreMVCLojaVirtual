﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LojaVirtual.Libraries.Email;
using LojaVirtual.Libraries.Filtro;
using LojaVirtual.Libraries.Lang;
using LojaVirtual.Libraries.Login;
using LojaVirtual.Libraries.Seguranca;
using LojaVirtual.Models;
using LojaVirtual.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LojaVirtual.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    public class HomeController : Controller
    {
        private IEnderecoEntregaRepository _repositoryEnderecoEntrega;
        private IClienteRepository _repositoryCliente;
        private LoginCliente _loginCliente;
        private GerenciarEmail _gerenciarEmail;
        private string passPhrase = "xlajwoijawfoagubi125";

        public HomeController(IEnderecoEntregaRepository repositoryEnderecoEntrega, IClienteRepository repositoryCliente, LoginCliente loginCliente, GerenciarEmail gerenciarEmail)
        {
            _repositoryEnderecoEntrega = repositoryEnderecoEntrega;
            _repositoryCliente = repositoryCliente;
            _loginCliente = loginCliente;
            _gerenciarEmail = gerenciarEmail;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromForm]Models.Cliente cliente, string returnUrl = null)
        {
            Models.Cliente clienteDB = _repositoryCliente.Login(cliente.Email, cliente.Senha);

            if (clienteDB != null)
            {
                _loginCliente.Login(clienteDB);

                if(returnUrl == null)
                {
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
                else
                {
                    return LocalRedirectPermanent(returnUrl);
                }                
            }
            else
            {
                ViewData["MSG_E"] = "Usuário não encontrado, verifique o e-mail e senha digitado!";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Recuperar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Recuperar([FromForm]Models.Cliente cliente)
        {
            ModelState.Remove("Nome");
            ModelState.Remove("Nascimento");
            ModelState.Remove("Sexo");
            ModelState.Remove("CPF");
            ModelState.Remove("Telefone");
            ModelState.Remove("CEP");
            ModelState.Remove("Estado");
            ModelState.Remove("Cidade");
            ModelState.Remove("Bairro");
            ModelState.Remove("Endereco");
            ModelState.Remove("Complemento");
            ModelState.Remove("Numero");
            ModelState.Remove("Senha");
            ModelState.Remove("ConfirmacaoSenha");

            if (ModelState.IsValid) {
                var clienteDoBancoDados = _repositoryCliente.ObterClientePorEmail(cliente.Email);

                if(clienteDoBancoDados != null)
                {
                    string idCrip = StringCipher.Encrypt(clienteDoBancoDados.Id.ToString(), passPhrase);
                    _gerenciarEmail.EnviarLinkResetarSenha(clienteDoBancoDados, idCrip);

                    TempData["MSG_S"] = Mensagem.MSG_S004;
                }
                else
                {
                    TempData["MSG_E"] = Mensagem.MSG_E014;
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult CriarSenha(string id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult CriarSenha([FromForm]Models.Cliente cliente, string id)
        {
            ModelState.Remove("Nome");
            ModelState.Remove("Nascimento");
            ModelState.Remove("Sexo");
            ModelState.Remove("CPF");
            ModelState.Remove("Email");
            ModelState.Remove("Telefone");
            ModelState.Remove("CEP");
            ModelState.Remove("Estado");
            ModelState.Remove("Cidade");
            ModelState.Remove("Bairro");
            ModelState.Remove("Endereco");
            ModelState.Remove("Complemento");
            ModelState.Remove("Numero");

            if (ModelState.IsValid)
            {
                var idClienteDecrip = StringCipher.Decrypt(id, passPhrase);
                int idCliente;
                if (int.TryParse(idClienteDecrip, out idCliente))
                {
                    var clienteDB = _repositoryCliente.ObterCliente(idCliente);
                    if (clienteDB != null)
                    {
                        clienteDB.Senha = cliente.Senha;

                        _repositoryCliente.Atualizar(clienteDB);
                        TempData["MSG_S"] = Mensagem.MSG_S004;
                    }
                }
                else
                {
                    TempData["MSG_E"] = Mensagem.MSG_E015;
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult Sair()
        {
            _loginCliente.Logout();

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}