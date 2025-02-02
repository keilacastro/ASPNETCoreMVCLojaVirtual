﻿using LojaVirtual.Libraries.Seguranca;
using LojaVirtual.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LojaVirtual.Libraries.Email
{
    public class GerenciarEmail
    {
        private SmtpClient _smtp;
        private IConfiguration _configuration;
        private IHttpContextAccessor _httpContextAccessor;
        private string passPhrase = "xlajwoijawfoagubi125";


        public GerenciarEmail(SmtpClient smtp, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _smtp = smtp;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public void EnviarContatoPorEmail(Contato contato)
        {
            string corpoMsg = string.Format("<h2>Contato - LojaVirtual</h2>" +
                "<b>Nome: </b> {0} <br />" +
                "<b>E-mail: </b> {1} <br />" +
                "<b>Texto: </b> {2} <br />" +
                "<br /> E-mail enviado automaticamente do site LojaVirtual.",
                contato.Nome,
                contato.Email,
                contato.Texto
            );


            /*
             * MailMessage -> Construir a mensagem
             */
            MailMessage mensagem = new MailMessage();
            mensagem.From = new MailAddress(_configuration.GetValue<string>("Email:Username"));
            mensagem.To.Add("elias.ribeiro.s@gmail.com");
            mensagem.Subject = "Contato - LojaVirtual - E-mail: " + contato.Email;
            mensagem.Body = corpoMsg;
            mensagem.IsBodyHtml = true;

            //Enviar Mensagem via SMTP
            _smtp.Send(mensagem);
        }

        public void EnviarSenhaParaColaboradorPorEmail(Colaborador colaborador)
        {
            string corpoMsg = string.Format("<h2>Colaborador - LojaVirtual</h2>" +
                "Sua senha é:" +
                "<h3>{0}</h3>", colaborador.Senha);


            /*
             * MailMessage -> Construir a mensagem
             */
            MailMessage mensagem = new MailMessage();
            mensagem.From = new MailAddress(_configuration.GetValue<string>("Email:Username"));
            mensagem.To.Add(colaborador.Email);
            mensagem.Subject = "Colaborador - LojaVirtual - Senha do colaborador - " + colaborador.Nome;
            mensagem.Body = corpoMsg;
            mensagem.IsBodyHtml = true;

            //Enviar Mensagem via SMTP
            _smtp.Send(mensagem);
        }

        public void EnviarDadosDoPedido(Cliente cliente, Pedido pedido)
        {
            string corpoMsg = string.Format("<h2>Pedido - LojaVirtual</h2>" +
                
                "Pedido realizado com sucesso!<br />" +
                "<h3>Nº {0}</h3>" +
                "<br /> Faça o login em nossa loja virtual e acompanhe o andamento.",
                pedido.Id + "-" + pedido.TransactionId
                
            );


            /*
             * MailMessage -> Construir a mensagem
             */
            MailMessage mensagem = new MailMessage();
            mensagem.From = new MailAddress(_configuration.GetValue<string>("Email:Username"));
            mensagem.To.Add(cliente.Email);
            mensagem.Subject = "LojaVirtual - Pedido - " + pedido.Id + "-" + pedido.TransactionId;
            mensagem.Body = corpoMsg;
            mensagem.IsBodyHtml = true;

            //Enviar Mensagem via SMTP
            _smtp.Send(mensagem);
        }

        public void EnviarLinkResetarSenha(Cliente cliente)
        {
            string IdCrip = StringCipher.Encrypt( cliente.Id.ToString() , passPhrase);

            var request = _httpContextAccessor.HttpContext.Request;
            string url = $"{request.Scheme}://{request.Host}/Cliente/Home/CriarSenha/{IdCrip}";
            string corpoMsg = string.Format(
                "<h2>Criar nova Senha para {1}({2})</h2>" +
                "Clique no link abaixo para criar uma nova senha!<br />" +
                "<a href='{0}' target='_blank'>{0}</a>",
                url,
                cliente.Nome,
                cliente.Email
            );


            /*
             * MailMessage -> Construir a mensagem
             */
            MailMessage mensagem = new MailMessage();
            mensagem.From = new MailAddress(_configuration.GetValue<string>("Email:Username"));
            mensagem.To.Add(cliente.Email);
            mensagem.Subject = "LojaVirtual - Criar nova senha - " + cliente.Nome;
            mensagem.Body = corpoMsg;
            mensagem.IsBodyHtml = true;

            //Enviar Mensagem via SMTP
            _smtp.Send(mensagem);
        }
    }
}
