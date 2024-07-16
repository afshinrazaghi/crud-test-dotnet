using Asp.Versioning;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Commands;
using Mc2.CrudTest.Presentation.Application.Features.Customers.Queries;
using Mc2.CrudTest.Presentation.Server.Extensions;
using Mc2.CrudTest.Presentation.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Mc2.CrudTest.Presentation.Server.Controllers
{
    [ApiController]
    //[ApiVersion("1.0")]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody][Required] CreateCustomerCommand command, CancellationToken cancellationToken) =>
            (await _mediator.Send(command, cancellationToken)).ToActionResult();


        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody][Required] UpdateCustomerCommand command,  CancellationToken cancellationToken) =>
           (await _mediator.Send(command, cancellationToken)).ToActionResult();


        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromBody][Required] DeleteCustomerCommand command, CancellationToken cancellationToken) =>
           (await _mediator.Send(command, cancellationToken)).ToActionResult();


        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCustomers([FromQuery] GetAllCustomerQuery query, CancellationToken cancellationToken) =>
           (await _mediator.Send(query, cancellationToken)).ToActionResult();



        [HttpGet]
        [Route("GetCustomerByEmail")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomerByEmail([FromQuery] GetCustomerByEmailQuery query,  CancellationToken cancellationToken) =>
           (await _mediator.Send(query, cancellationToken)).ToActionResult();


    }
}

