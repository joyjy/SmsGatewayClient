package im.joyjy.test.sms.message;

import java.io.Serializable;

public class SmgpSmsMessage implements Serializable {
	private static final long serialVersionUID = 1L;

	private int requestId;
	private int sequenceId;
	private int packetLength;

	public int getRequestId() {
		return requestId;
	}

	public void setRequestId(int requestId) {
		this.requestId = requestId;
	}

	public int getSequenceId() {
		return sequenceId;
	}

	public void setSequenceId(int sequenceId) {
		this.sequenceId = sequenceId;
	}

	public int getPacketLength() {
		return packetLength;
	}

	public void setPacketLength(int packetLength) {
		this.packetLength = packetLength;
	}

	public String toString() {
		return "[PacketLength=" + packetLength + ",RequestId=" + requestId
				+ ",SequenceId=" + sequenceId + "]";
	}
}
