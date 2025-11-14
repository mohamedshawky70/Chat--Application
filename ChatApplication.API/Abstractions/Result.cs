using System;

namespace ChatApplication.API.Abstractions;

public class Result
{
	public bool IsSuccess { get;}
	public bool IsFailure => !IsSuccess;
	public Error Error { get;}=default!;

	public Result(bool isSuccess, Error error)
	{
		//فشل ومفيش ايرور او نجح وفيه ايرور مش منطقي
		if((isSuccess && error!=Error.None) || (!isSuccess && error == Error.None))
			throw new InvalidOperationException();

		IsSuccess = isSuccess;
		Error = error;
	}
	
	public static Result Success()
	{
		return new Result(true, Error.None);
	}
	
	public static Result Failure(Error error)
	{
		return new Result(false, error);
	}

	// Two methods for generic Result<TValue> to reutrn success with value
	public static Result<TValue> Success<TValue>(TValue value)
	{
		return new Result<TValue>(value,true, Error.None);
	}

	public static Result<TValue> Failure<TValue>(Error error)
	{
		return new Result<TValue>(default,false, error);
	}
}



// To return success with value
public class Result<TValue> : Result
{
	private readonly TValue? _value ;
	public Result(TValue? value, bool isSuccess, Error error):base(isSuccess, error)
	{
		_value = value;
	}
	public TValue Value()
	{
		return IsSuccess? 
				_value! 
			: throw new InvalidOperationException("Failure result connot have value");
	}
}
