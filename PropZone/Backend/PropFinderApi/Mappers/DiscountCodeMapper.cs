using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Mappers
{
    public class DiscountCodeMapper
    {
        public DiscountCodeDto MapToDto(DiscountCode discount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            return new DiscountCodeDto
            {
                Id = discount.Id,
                Code = discount.Code,
                DiscountValue = discount.DiscountValue,
                IsPercentage = discount.IsPercentage,
                FromDate = discount.FromDate,
                ToDate = discount.ToDate,
                IsActive = discount.IsActive,
                IsDeleted = discount.IsDeleted,
                MaxListerLimit = discount.MaxListerLimit,
                Options =
                    discount
                        .Options?.Select(o => new DiscountCodeOptionsDto
                        {
                            TypeOfProperty = o.TypeOfProperty,
                            PurposeOfListing = o.PurposeOfListing,
                            MinPrice = o.MinPrice,
                            MaxPrice = o.MaxPrice,
                        })
                        .ToList() ?? new List<DiscountCodeOptionsDto>(),
            };
        }

        public DiscountCode MapToEntity(DiscountCodeAddRequestDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return new DiscountCode
            {
                Id = Guid.NewGuid(),
                Code = dto.Code,
                DiscountValue = dto.DiscountValue,
                IsPercentage = dto.IsPercentage,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                IsActive = dto.IsActive,
                IsDeleted = false,
                MaxListerLimit = dto.MaxListerLimit,
                Options =
                    dto.Options?.Select(o => new DiscountCodeOptions
                        {
                            Id = Guid.NewGuid(),
                            TypeOfProperty = o.TypeOfProperty,
                            PurposeOfListing = o.PurposeOfListing,
                            MinPrice = o.MinPrice,
                            MaxPrice = o.MaxPrice,
                        })
                        .ToList() ?? new List<DiscountCodeOptions>(),
            };
        }

        public DiscountCode MapUpdateDto(
            DiscountCodeUpdateRequestDto dto,
            DiscountCode existingCode
        )
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (existingCode == null)
                throw new ArgumentNullException(nameof(existingCode));

            existingCode.Code = dto.Code ?? existingCode.Code;
            existingCode.DiscountValue = dto.DiscountValue ?? existingCode.DiscountValue;
            existingCode.IsPercentage = dto.IsPercentage ?? existingCode.IsPercentage;
            existingCode.FromDate = dto.FromDate ?? existingCode.FromDate;
            existingCode.ToDate = dto.ToDate ?? existingCode.ToDate;
            existingCode.IsActive = dto.IsActive ?? existingCode.IsActive;
            existingCode.MaxListerLimit = dto.MaxListerLimit ?? existingCode.MaxListerLimit;

            if (dto.Options != null)
            {
                existingCode.Options = dto
                    .Options.Select(o => new DiscountCodeOptions
                    {
                        TypeOfProperty = o.TypeOfProperty,
                        PurposeOfListing = o.PurposeOfListing,
                        MinPrice = o.MinPrice,
                        MaxPrice = o.MaxPrice,
                    })
                    .ToList();
            }

            return existingCode;
        }
    }
}
