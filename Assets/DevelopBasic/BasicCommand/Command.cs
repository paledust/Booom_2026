using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CommandStatus{Detached, Pending, Working, Success, Fail, Aborted}
public abstract class Command<T>{
	private Action onCompleteCallBack;
    public CommandStatus Status = CommandStatus.Detached;
    public bool IsDetached{get{return Status == CommandStatus.Detached;}}
    public bool IsPending{get{return Status == CommandStatus.Pending;}}
    public bool IsWorking{get{return Status == CommandStatus.Working;}}
    public bool IsSuccess{get{return Status == CommandStatus.Success;}}
    public bool IsFail{get{return Status == CommandStatus.Fail;}}
    public bool IsAborted{get{return Status == CommandStatus.Aborted;}}    
    public bool IsDone{get{return (Status == CommandStatus.Fail || Status == CommandStatus.Success || Status == CommandStatus.Aborted);}}
    protected Command<T> nextCommand = null;
	//Make sure the command always added to the last
	public Command<T> QueueCommand(Command<T> command){
		if(nextCommand==null){
			nextCommand = command;
		}
		else{
			nextCommand.QueueCommand(command);
		}
		return command;
	}
	public Command<T> GetNextCommand(){return nextCommand;}
	public Command<T> OnCommandComplete(Action callback){
		onCompleteCallBack = callback;
		return this;
	}
    internal void SetStatus(CommandStatus newStatus){
		if (Status == newStatus) return;

		Status = newStatus;

		switch (newStatus){
			case CommandStatus.Working:
				Init();
				break;
			case CommandStatus.Success:
				OnSuccess();
				CleanUp();
				break;
			case CommandStatus.Aborted:
				OnAbort();
				CleanUp();
				break;
			case CommandStatus.Fail:
				OnFail();
				CleanUp();
				break;
			case CommandStatus.Detached:
			case CommandStatus.Pending:
				break;
			default:
				Debug.Log("None status is found");
				Debug.Assert(false);
				break;
		}
	}
	internal virtual void CommandUpdate(T context){}
	protected virtual void Init(){}
	protected virtual void OnAbort(){}
	protected virtual void OnSuccess(){onCompleteCallBack?.Invoke();}
	protected virtual void OnFail(){}
	protected virtual void CleanUp(){}
}

public class C_Wait<T>:Command<T>{
	public float waitTime = 0;
	public C_Wait(float _time){
		waitTime = _time;
	}
    internal override void CommandUpdate(T context)
    {
        base.CommandUpdate(context);

		waitTime -= Time.deltaTime;
		if(waitTime<=0) SetStatus(CommandStatus.Success);
    }
}