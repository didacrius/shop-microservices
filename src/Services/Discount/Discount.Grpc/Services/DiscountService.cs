using AutoMapper;
using Dapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories.Interfaces;
using Grpc.Core;

namespace Discount.Grpc.Services;

public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
{
    private readonly IDiscountRepository repository;
    private readonly IMapper mapper;
    private readonly ILogger<DiscountService> logger;

    public DiscountService(IDiscountRepository repository, ILogger<DiscountService> logger, IMapper mapper)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var coupon = await this.repository.GetDiscount(request.ProductName);
        if (coupon is null)
        {
            throw new RpcException(
                new Status(StatusCode.NotFound,
                    $"Discount with ProductName={request.ProductName} is not found."));
        }
        
        logger.LogInformation("Discount is retrieved for Product Name: {ProductName}," +
                              " Amount: {Amount}", coupon.ProductName, coupon.Amount);

        var couponModel = mapper.Map<CouponModel>(coupon);
        return couponModel;
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var coupon = mapper.Map<Coupon>(request.Coupon);

        var success = await repository.CreateDiscount(coupon);
        if (success is false)
        {
            throw new RpcException(
                new Status(StatusCode.Internal,
                    $"Discount is not created."));
        }
        
        logger.LogInformation("Discount is successfully created." +
                              " Product Name: {ProductName}", coupon.ProductName);

        var couponModel = mapper.Map<CouponModel>(coupon);
        return couponModel;
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var coupon = mapper.Map<Coupon>(request.Coupon);

        var success = await repository.UpdateDiscount(coupon);
        if (success is false)
        {
            throw new RpcException(
                new Status(StatusCode.NotFound,
                    $"Discount not found."));
        }
        
        logger.LogInformation("Discount is successfully created." +
                              " Product Name: {ProductName}", coupon.ProductName);

        var couponModel = mapper.Map<CouponModel>(coupon);
        return couponModel;
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        var success = await repository.DeleteDiscount(request.ProductName);
        var response = new DeleteDiscountResponse
        {
            Success = success
        };

        return response;
    }
}