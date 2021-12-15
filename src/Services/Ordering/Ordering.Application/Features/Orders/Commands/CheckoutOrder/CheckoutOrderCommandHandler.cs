using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder;

public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly ILogger<CheckoutOrderCommandHandler> _logger;

    public CheckoutOrderCommandHandler(IOrderRepository repository, IMapper mapper, IEmailService emailService,
        ILogger<CheckoutOrderCommandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        var order = this._mapper.Map<Order>(request);
        var createdOrder = await this._repository.AddAsync(order);
        
        _logger.LogInformation("Order {Id} is successfully created", createdOrder.Id);

        await SendEmail(createdOrder);

        return createdOrder.Id;
    }

    private async Task SendEmail(Order order)
    {
        var email = new Email
        {
            To = "didacriuscom@gmail.com",
            Body = "Order was created",
            Subject = "New order"
        };

        try
        {
            await _emailService.Send(email);
        }
        catch (Exception e)
        {
            _logger.LogError("Order {Id} failed due to an error with the email service: {Exception}",
                order.Id, e.Message);
        }
    }
}