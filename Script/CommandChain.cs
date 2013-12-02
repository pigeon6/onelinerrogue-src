using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CommandChain : ScriptableObject {

	public delegate void ChainCommand(object[] args, CommandChain cc);

	public struct Command {
		ChainCommand 	func;
		object[]		args;

		public Command(ChainCommand f) {
			func = f;
			args = null;
		}
		public Command(ChainCommand f, object a0) {
			func = f;
			args = new object[1];
			args[0] = a0;
		}
		public Command(ChainCommand f, object a0, object a1) {
			func = f;
			args = new object[2];
			args[0] = a0;
			args[1] = a1;
		}
		public Command(ChainCommand f, object a0, object a1, object a2) {
			func = f;
			args = new object[3];
			args[0] = a0;
			args[1] = a1;
			args[2] = a2;
		}
		public Command(ChainCommand f, object a0, object a1, object a2, object a3) {
			func = f;
			args = new object[4];
			args[0] = a0;
			args[1] = a1;
			args[2] = a2;
			args[3] = a3;
		}

		public void FireCommand(CommandChain cc) {
			func(args, cc);
		}
	}

	[SerializeField]
	private LinkedList<Command> m_queue;

	public void Add(Command c) {
		if( m_queue == null ) {
			m_queue = new LinkedList<Command>();
		}
		m_queue.AddLast(c);
	}

	public void InturrptAdd(Command c) {
		if( m_queue == null ) {
			m_queue = new LinkedList<Command>();
		}
		m_queue.AddFirst(c);
	}

	public void FireCommand() {
		if( m_queue != null && m_queue.Count > 0 ) {
			Command c = m_queue.First.Value;
			m_queue.RemoveFirst();
			c.FireCommand(this);
		}
	}
}
