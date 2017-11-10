﻿namespace Hvs.Repositories
{
	using System.Data.Entity;
	using System.Data.Entity.Validation;
	using System.Net;
	using System.Threading.Tasks;
	using Common.Api;
	using Interfaces.Architecture;
	using Microsoft.EntityFrameworkCore;
	using EntityState = Microsoft.EntityFrameworkCore.EntityState;

	public abstract class CrudRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
	{
		protected CrudRepository(HvsDbContext dbContext)
		{
			DbContext = dbContext;
		}

		protected HvsDbContext DbContext { get; }
		public abstract System.Data.Entity.DbSet<TEntity> DbSet { get; }
		public abstract string ClassName { get; }

		public virtual async Task<ApiDataResponce<TEntity>> GetById(long id)
		{
			var (result, _) = await FindById(id);
			return result;
		}

		public virtual async Task<ApiDataListResponce<TEntity>> Get(IFilter<TEntity> filter)
		{
			if (filter == null)
			{
				return new ApiDataListResponce<TEntity>(await EntityFrameworkQueryableExtensions.ToListAsync(DbSet),
					await DbSet.LongCountAsync());
			}
			return filter.Apply();
		}

		public virtual async Task<ApiDataResponce<TEntity>> Create(TEntity entity)
		{
			if (entity == null)
			{
				return new ApiDataResponce<TEntity>(HttpStatusCode.BadRequest,
					$"{ClassName} cannot be created because there are no data provided!");
			}

			TEntity result = DbSet.Add(entity);
			try
			{
				await DbContext.SaveChangesAsync();
			}
			catch (DbEntityValidationException ex)
			{
				return new ApiDataResponce<TEntity>(HttpStatusCode.InternalServerError, ex.ToString());
			}

			return new ApiDataResponce<TEntity>(result);
		}

		public virtual async Task<ApiDataResponce<TEntity>> Update(long id, TEntity entity)
		{
			if (entity == null)
			{
				return new ApiDataResponce<TEntity>(HttpStatusCode.BadRequest,
					$"{ClassName} cannot be updated because there are no data provided!");
			}
			var (existingEntityResult, isExists) = await FindById(id);
			if (isExists)
			{
				return existingEntityResult;
			}
			TEntity existingEntity = existingEntityResult.Data;
			Merge(entity, existingEntity);
			DbContext.Entry(existingEntity).State = EntityState.Modified;
			try
			{
				await DbContext.SaveChangesAsync();
			}
			catch (DbEntityValidationException ex)
			{
				return new ApiDataResponce<TEntity>(HttpStatusCode.InternalServerError, ex.ToString());
			}
			return new ApiDataResponce<TEntity>(existingEntity);
		}

		public virtual async Task<ApiResponse> Remove(long id)
		{
			var (entityResult, isExists) = await FindById(id);
			if (isExists)
			{
				return entityResult;
			}
			DbSet.Remove(entityResult.Data);
			await DbContext.SaveChangesAsync();
			return new ApiResponse();
		}

		protected abstract void Merge(TEntity source, TEntity destination);

		private async Task<(ApiDataResponce<TEntity>, bool)> FindById(long id)
		{
			TEntity entity = await DbSet.FindAsync(id);
			return entity == null
				? (new ApiDataResponce<TEntity>(HttpStatusCode.NotFound, $"Entity {ClassName} (id = {id}) is not found"), false)
				: (new ApiDataResponce<TEntity>(entity), true);
		}
	}
}