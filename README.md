# TPLConsoleApp
What is TPL Dataflow?
With Reference from https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library

The Task Parallel Library (TPL) provides dataflow components to help increase the robustness of concurrency-enabled applications. These dataflow components are collectively referred to as the TPL Dataflow Library.  These dataflow components are useful when you have multiple operations that must communicate with one another asynchronously or when you want to process data as it becomes available. For example, consider an application that processes image data from a web camera. By using the dataflow model, the application can process image frames as they become available. If the application enhances image frames, for example, by performing light correction or red-eye reduction, you can create a pipeline of dataflow components. Each stage of the pipeline might use more coarse-grained parallelism functionality, such as the functionality that is provided by the TPL, to transform the image.

 


Types of Blocks
The TPL Dataflow Library consists of dataflow blocks, which are data structures that buffer and process data. The TPL defines three kinds of dataflow blocks: source blocks, target blocks, and propagator blocks.


Source Block
A source block acts as a source of data and can be read from. A block implementing

System.Threading.Tasks.Dataflow.ISourceBlock<T Output> interface  represents a source.

Examples:

BufferBlock, TransformBlock, TransformManyBlock,BatchBlock etc.


Target Block
A target block acts as a receiver of data and can be written to. A block implementing System.Threading.Tasks.Dataflow.ITargetBlock<TInput> interface represents a target.

Examples:

ActionBlock(can act only as Target Block), BufferBlock, TransformBlock, TransformManyBlock,

BatchBlock etc.

 


Propagator Block
A propagator block acts as both a source block and a target block, and can be read from and written to. A block implementing System.Threading.Tasks.Dataflow.IPropagatorBlock<TInput,TOutput> interface represents propagators. IPropagatorBlock<TInput,TOutput> inherits from both ISourceBlock<TOutput>, and ITargetBlock<TInput>.

Example:

BufferBlock, TransformBlock, TransformManyBlock, BatchBlock etc.

Both ISourceBlock<TOutput> and ITargetBlock<TInput> inherit the IDataflowBlock interface.

 


IDataflowBlock Interface
A class implementing this interface represents a dataflow block. ISourceBlock<TInput> and ITargetBlock<TOutput> both inherits from IDataflowBlock.


Properties:
Task Completion                It returns a Task that represents the asynchronous operation and completion of 

                                           the dataflow block.

 


Methods:
void Complete ()               Signals to the IDataflowBlock that it should not accept nor produce any more 

                                           messages nor consume any more postponed messages.

 

void Fault (Exception exception)  Causes the IDataflowBlock to complete in a Faulted state.

Parameters: The System.Exception that caused the faulting.

 


ISourceBlock<TOutput> Interface
Represents a dataflow block that is a source of data. It Inherits from IDataflowBlock. In addition to the properties and methods of IDataflowBlock, ISourceBlock has following extra methods:


Methods:
TOutput ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target, out bool messageConsumed)

Description: Called by a linked ITargetBlock<TInput> to accept and consume a DataflowMessageHeader previously offered by this ISourceBlock<TOutput>.    

 

LinkTo(ITargetBlock<TOutput>, DataflowLinkOptions) 

Descriptions: Links the System.Threading.Tasks.Dataflow.ISourceBlock`1 to the specified System.Threading.Tasks.Dataflow.ITargetBlock`1.

It returns an IDisposable that, upon calling Dispose, will unlink the source from the target.

ITargetBlock is the block to which we want to link this source block.

DataflowLinkOptions is used to configure the link.

 

Void ReleaseReservation (DataflowMessageHeader, ITargetBlock<TOutput>)

Description: Called by a linked ITargetBlock<TInput> to release a previously reserved DataflowMessageHeader by this ISourceBlock<TOutput>.

 

bool ReserveMessage (DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target)

Description: Called by a linked ITargetBlock<TInput> to reserve a previously offered DataflowMessageHeader by this ISourceBlock<TOutput>. 

 

Receive<TOutput>(ISourceBlock<TOutput>)

Description: Synchronously receives a value from a specified source.

 

Receive<TOutput>(ISourceBlock<TOutput>, CancellationToken)

Description: Asynchronously receives a value from a specified source and provides a token to cancel the operation.

 

ReceiveAsync<TOutput>(ISourceBlock<TOutput>)

Description: Asynchronously receives a value from a specified source.

 

ReceiveAsync<TOutput>(ISourceBlock<TOutput>, CancellationToken)

Description: Asynchronously receives a value from a specified source and provides a token to cancel the operation.

 


ITargetBlock<TInput> Interface
Represents a dataflow block that is a target for data. In addition to the properties and methods of IDataflowBlock, ISourceBlock has following extra methods:


Methods:
OfferMessage(DataflowMessageHeader, TInput, ISourceBlock<TInput>, Boolean)

Description: Offers a message to the ITargetBlock<TInput>, giving the target the opportunity to consume or postpone the message.

 

Post<TInput>(ITargetBlock<TInput>, TInput)

Description: Posts an item to the ITargetBlock<TInput>.

 

SendAsync<TInput>(ITargetBlock<TInput>, TInput)

Description: Asynchronously offers a message to the target message block, allowing for postponement.

 

SendAsync<TInput>(ITargetBlock<TInput>, TInput, CancellationToken)

Description: Asynchronously offers a message to the target message block, allowing for postponement and cancellation option.

 


DataflowBlockOptions Class
This Class provides options used to configure the processing performed by dataflow blocks.


Fields:
Unbounded                           
A constant used to specify an unlimited quantity for DataflowBlockOptions members that provide an upper bound. This field is constant.


Properties:
Name

Description

BoundedCapacity

Gets or sets the maximum number of messages that may be buffered by the block.

CancellationToken

Gets or sets the CancellationToken to monitor for cancellation requests.

EnsureOrdered

Gets or sets a value that indicates whether ordered processing should be enforced on a block's handling of messages.

MaxMessagesPerTask

Gets or sets the maximum number of messages that may be processed per task.

NameFormat

Gets or sets the format string to use when a block is queried for its name.

TaskScheduler

Gets or sets the TaskScheduler to use for scheduling tasks.

 


ExecutionDataflowBlockOptions Class
It inherits from DataflowBlockOptions class. Provides options used to configure the processing performed by dataflow blocks that process each message through the invocation of a user-provided delegate. These are dataflow blocks such as ActionBlock<TInput> and TransformBlock<TInput,TOutput>. In addition to the fields and properties of DataflowBlockOptions, it has the following properties:

Name

Description

MaxDegreeOfParallelism

Gets the maximum number of messages that may be processed by the block concurrently.

SingleProducerConstrained

Gets whether code using the dataflow block is constrained to one producer at a time.

 


Predefined Dataflow Block Types
The TPL Dataflow Library provides several predefined dataflow block types. These types are divided into three categories: buffering blocks, execution blocks, and grouping blocks.


Buffering Blocks
Buffering blocks hold data for use by data consumers. The TPL Dataflow Library provides three buffering block types: 

System.Threading.Tasks.Dataflow.BufferBlock<T>,                            System.Threading.Tasks.Dataflow.BroadcastBlock<T>, and System.Threading.Tasks.Dataflow.WriteOnceBlock<T>.

BroadcastBlock<T>

This Block Inherit from IDataflowBlock, IPropagatorBlock<TInput, TOutput>,ITargetBlock<TInput>,ISourceBlock<TOutput> and IReceivableSourceBlock<T>.

It implements all the methods of IDataflowBlock, ITargetBlock and

ISourceBlock.

The BroadcastBlock<T> class is useful when you must pass multiple messages to another component, but that component needs only the most recent value.

It acts as a buffer for storing at most one element at time, overwriting each message with the next as it arrives.

This class is also useful when you want to broadcast a message to multiple components.

 Unlike other blocks, it continue to offer the same message to all linked targets ,even if it has been accepted by one of the linked blocks.

We can Post the messages to broadcast block synchronously using Post() method or asynchronously using SendAsync() method.

In both cases, the block will continue to accept input without any postponing of messages as by virtue of its behavior it simply overwrites the existing message with the new incoming message.

As soon as we call complete() method on this block, it will stop accepting the incoming messages and stop offering the existing message to the linked target irrespective of whether existing message was offered to the target at least once. 

We could declare a BroadcastBlock in the following way:

public BroadcastBlock(Func<T, T> cloningFunction)

Func<T,T> is the function which will clone the input message passed to this block and returns the clone message

public BroadcastBlock(Func<T, T> cloningFunction, DataflowBlockOptions dataflowBlockOptions);

 DataflowBlockOptions is use to configure the processing performed by the block.

 

BufferBlock<T>

This Block Inherit from IDataflowBlock, IPropagatorBlock<T, T>,IReceivableSourceBlock<T>.

This class stores a first in, first out (FIFO) queue of messages that can be written to by multiple sources or read from by multiple targets. 

When a target receives a message from a BufferBlock<T> object, that message is removed from the message queue. 

Therefore, although a BufferBlock<T> object can have multiple targets, only one target will receive each message i.e.

In case the Buffer block is linked to multiple targets and the message has been accepted by one target block, then that message will NOT be offered to other linked targets as it gets removed from the output que .

If a target does not accept the message, the buffer block will continue to offer that message to other linked targets.

If none of the linked targets accepts the message, then the block will stop

offering next message to the targets as that unconsumed message still exist in que and cannot be overwritten or removed unless it is passed or manually received from its output que.

As soon as we call complete() method on this block, it will stop accepting the incoming messages but will wait until all the messages from its output que are consumed by the linked target blocks.

We can Post the messages to buffer block synchronously using Post() method or asynchronously using SendAsync() method.

If we use Post() method to add message to block, then if block postpone or decline the message ,in both cases the method will return false and will drop that message and move on to offering the next message.

If we use SendAsync() method along with awaiting to add message to block, then the method will wait asynchronously  till either the message is accepted or declined.

We could declare a BufferBlock in the following ways:

Var buffer = new BufferBlock<T>( )

Var buffer = new BufferBlock<T>(new DataflowBlockOptions());

 

WriteOnceBlock<T>

This Block Inherit from IDataflowBlock, IPropagatorBlock<T, T>,IReceivableSourceBlock<T>

It provides a buffer for receiving and storing at most one element in a network of dataflow blocks.

The WriteOnceBlock<T> class resembles the BroadcastBlock<T> class, except that a WriteOnceBlock<T> object can be written to one time only.

Like the BroadcastBlock<T> class, when a target receives a message from a WriteOnceBlock<T> object, multiple targets receive a copy of the message.

The WriteOnceBlock<T> class is useful when you want to propagate only the first of multiple messages. 

Once the first message is added to this block, it stops accepting any other incoming messages.

We don’t need to call complete() method on this block as it automatically stop accepting the incoming messages past the addition of first message to it.

Therefore , both Post() and SendAsync() method will return false on addition of any message to the block except the first message.

Hence ,It can move itself to completion state without calling the complete() method on it.

We could declare a WriteOnceBlock using the following constructors:

public WriteOnceBlock(Func<T, T> cloningFunction)

public WriteOnceBlock(Func<T, T> cloningFunction, DataflowBlockOptions dataflowBlockOptions)


Execution Blocks
Execution blocks call a user-provided delegate for each piece of received data. The TPL Dataflow Library provides three execution block types: ActionBlock<TInput>, TransformBlock<TInput,TOutput>, and TransformManyBlock<TInput,TOutput>.

ActionBlock<T>

This Block Inherit from IDataflowBlock, IPropagatorBlock<T, T>,IReceivableSourceBlock<T>.

The ActionBlock<TInput> class is a target block that calls a delegate when it receives data.

Think of a ActionBlock<TInput> object as a delegate that runs asynchronously when data becomes available. 

We could control the Degree of Parallelism by specifying the MaxDegreeOfParallelism  property of ExecutionDataflowBlockOptions which sets the allowed maximum number of tasks that this block could create for parallel execution.

We could also set the maximum number of messages served by a task by specifying the MaxMessagesPerTask property of DataflowBlockOptions.

We could set the maximum number of messages can be  buffered through the block by setting the BoundedCapacity property of DataflowBlockOptions . This Capacity is inclusive of the number of messages being currently in processing and the messages in the input buffer.

As soon as we call complete() method on this block, it will stop accepting the incoming messages but will process all the messages currently present in the input buffer before moving to the completion state.

We could declare a ActionBlock using the following constructors::

public ActionBlock(Action<TInput> action)

public ActionBlock(Func<TInput, Task> action)

public ActionBlock(Action<TInput> action, ExecutionDataflowBlockOptions dataflowBlockOptions)

public ActionBlock(Func<TInput, Task> action, ExecutionDataflowBlockOptions dataflowBlockOptions)

 

TransformBlock<TInput,TOutput>

The TransformBlock<TInput,TOutput> class resembles the ActionBlock<TInput> class, except that it acts as both a source and as a target.

The delegate that you pass to a TransformBlock<TInput,TOutput> object returns a value of type TOutput.

Its ideal for a scenario in which we want take input of data, process it and output the processed data.

One Input corresponds to one output.

We could control the Degree of Parallelism by specifying the MaxDegreeOfParallelism  property of ExecutionDataflowBlockOptions which sets the allowed maximum number of tasks that this block could create for parallel execution.

We could also set the maximum number of messages served by a task by specifying the MaxMessagesPerTask property of DataflowBlockOptions.

We could set the maximum number of messages can be  buffered through the block by setting the BoundedCapacity property of DataflowBlockOptions . This Capacity is inclusive of the number of messages being currently in processing, the messages in the input buffer and the messages in the output buffer.

As soon as we call complete() method on this block, it will stop accepting the incoming messages but will process all the messages currently present in the input buffer and will wait until all the messages get consumed from its output buffer before moving to the completion state.

We could declare a TransformBlock using the following constructors:

public TransformBlock(Func<TInput, Task<TOutput>> transform)

public TransformBlock(Func<TInput, TOutput> transform)

public TransformBlock(Func<TInput, Task<TOutput>> transform, ExecutionDataflowBlockOptions dataflowBlockOptions)

public TransformBlock(Func<TInput, TOutput> transform, ExecutionDataflowBlockOptions dataflowBlockOptions)

 

TransformManyBlock<TInput,TOutput>

The TransformManyBlock<TInput,TOutput> class resembles the TransformBlock<TInput,TOutput> class, except that TransformManyBlock<TInput,TOutput> produces zero or more output values for each input value, instead of only one output value for each input value. 

Output type of TransformManyBlock is IEnumerable<Toutput>.

Although it gives IEnumerable<T> as output corresponding to an input, it passes each element of IEnumerable<T> one by one to next Linked block and NOT as a whole IEnumerable<T> at once.

We could control the Degree of Parallelism by specifying the MaxDegreeOfParallelism  property of ExecutionDataflowBlockOptions which sets the allowed maximum number of tasks that this block could create for parallel execution.

We could also set the maximum number of messages served by a task by specifying the MaxMessagesPerTask property of DataflowBlockOptions.

We could set the maximum number of messages can be  buffered through the block by setting the BoundedCapacity property of DataflowBlockOptions . This Capacity is inclusive of the number of messages being currently in processing, the messages in the input buffer and the number of arrays each corresponding to a message in the output buffer.

As soon as we call complete() method on this block, it will stop accepting the incoming messages but will process all the messages currently present in the input buffer and will wait until all the messages get consumed from its output buffer before moving to the completion state.

We could declare a TransformManyBlock using the following constructors:

public TransformManyBlock(Func<TInput, IEnumerable<TOutput>> transform);

public TransformManyBlock(Func<TInput, Task<IEnumerable<TOutput>>> transform)

public TransformManyBlock(Func<TInput, IEnumerable<TOutput>> transform, ExecutionDataflowBlockOptions dataflowBlockOptions)

public TransformManyBlock(Func<TInput, Task<IEnumerable<TOutput>>> transform, ExecutionDataflowBlockOptions dataflowBlockOptions)

 

 


Grouping Blocks
Grouping blocks combine data from one or more sources and under various constraints. The TPL Dataflow Library provides three join block types: BatchBlock<T>, JoinBlock<T1,T2>, and BatchedJoinBlock<T1,T2>.

BatchBlock<T>

The BatchBlock<T> class combines sets of input data, which are known as batches, into arrays of output data.

You specify the size of each batch when you create a BatchBlock<T> object.

When the BatchBlock<T> object receives the specified count of input elements, it asynchronously propagates out an array that contains those elements. 

If a BatchBlock<T> object is set to the completed state but does not contain enough elements to form a batch, it propagates out a final array that contains the remaining input elements.

The BatchBlock<T> class operates in either greedy or non-greedy mode.

In greedy mode, which is the default, a BatchBlock<T> object accepts every message that it is offered and propagates out an array after it receives the specified count of elements.

 In non-greedy mode, a BatchBlock<T> object postpones all incoming messages until enough sources have offered messages to the block to form a batch.

Greedy mode typically performs better than non-greedy mode because it requires less processing overhead. However, you can use non-greedy mode when you must coordinate consumption from multiple sources in an atomic fashion. 

Specify non-greedy mode by setting Greedy to False in the dataflowBlockOptions parameter in the BatchBlock<T> constructor.

We could declare a BatchBlock using the following constructors:

public BatchBlock(int batchSize)

public BatchBlock(int batchSize, GroupingDataflowBlockOptions dataflowBlockOptions)

 

JoinBlock<T1,T2,….>

The JoinBlock<T1,T2> and JoinBlock<T1,T2,T3> classes collect input elements and propagate out System.Tuple<T1,T2> or System.Tuple<T1,T2,T3> objects that contain those elements.

The JoinBlock<T1,T2> and JoinBlock<T1,T2,T3> classes do not inherit from ITargetBlock<TInput>. 

Instead, they provide properties, Target1, Target2, and Target3, that implement ITargetBlock<TInput>.

Like BatchBlock<T>, JoinBlock<T1,T2> and JoinBlock<T1,T2,T3> operate in either greedy or non-greedy mode.

In greedy mode, which is the default, a JoinBlock<T1,T2> or JoinBlock<T1,T2,T3> object accepts every message that it is offered and propagates out a tuple after each of its targets receives at least one message.

In non-greedy mode, a JoinBlock<T1,T2> or JoinBlock<T1,T2,T3> object postpones all incoming messages until all targets have been offered the data that is required to create a tuple.

This postponement makes it possible for another entity to consume the data in the meantime, to allow the overall system to make forward progress.

 We could declare a JoinBlock using the following constructors:

Public JoinBlock<T1, T2, T3>()

Public JoinBlock<T1, T2, T3>(GroupingDataflowBlockOptions dataflowBlockOptions)

 

BatchedJoinBlock(T1, T2, ...)

The BatchedJoinBlock<T1,T2> and BatchedJoinBlock<T1,T2,T3> classes collect batches of input elements and propagate out System.Tuple(IList(T1), IList(T2)) or System.Tuple(IList(T1), IList(T2), IList(T3)) objects that contain those elements. 

Specify the size of each batch when you create a BatchedJoinBlock<T1,T2> object.

BatchedJoinBlock<T1,T2> also provides properties, Target1 and Target2, that implement ITargetBlock<TInput>.

When the specified count of input elements are received from across all targets, the BatchedJoinBlock<T1,T2> object asynchronously propagates out a System.Tuple(IList(T1), IList(T2)) object that contains those elements.

It CANNOT be use in Non Greedy mode.

We could declare a BatchedJoinBlock using the following constructors:

public BatchedJoinBlock(int batchSize)

public BatchedJoinBlock(int batchSize, GroupingDataflowBlockOptions dataflowBlockOptions)

 


Blocks Completion
We could call IDataflowBlock.complete() method to signal  the DataflowBlock that it should not accept nor produce any more messages nor consume any more postponed messages. We could wait for the completion of block by  using IDataflowBlock.Completion property which returns a task representing asynchronous operation and completion of task. We could asynchronously wait for the completion using await. We could even specify the task to be triggered on the completion of block by using Continuewith() method of Task.

We could use it to notify the user on the successful completion of Pipeline. 

 


Block Linking
The ISourceBlock<TOutput>.LinkTo method links a source dataflow block to a target block.

We could even do the filtering of the messages by specifying the condition ,which when holds true the message is passed to the linked target.

for example:

firstBlock.LinkTo(secondBlock, n => n < 10);

Consider two blocks where  firstBlock is an ISourceBlock and secondBlock is a ITargetBlock. When the specified condition results into true, the value 'n' is passed on to the secondBlock where n is the message flowing through the pipeline and could be of any type.

We could configure the link using DataflowLinkOptions class. It has the following properties:

int MaxMessages : It sets the maximum number of messages that may be consumed across the link.

After the set number of messages have been consumed through the link, the target block gets unlinked from the source block.

bool PropagateCompletion : It tells the whether the linked target block will be notified about the completion, cancellation or fault status of its source block.

bool Append : Gets or sets whether the link should be appended to the source's list of links, or whether it should be prepended. That means , if the source of the link which we are configuring is linked to multiple targets and  if we set this property to False , then all the messages from the source block will be first offered to this block and if got postponed or declined, will be offered to other linked blocks. 


Cancellation of Blocks
We can make a block cancellable by passing a cancellation token to it during its construction as 

CancellationToken  property  of DataflowBlockOptions. Whenever we will call CancellationTokenSource.Cancel() method, It will make block to enter into Cancelled state.

Once cancellation is requested on a block, it will stop accepting the incoming messages and stop propagating messages to next block, even if the messages have been there in its input que.

If we will wait for the completion on blocks following the cancelled block, they will complete with status RanToCompletion. 

If we will wait for the completion on cancelled block , then it will throw TaskCancelledException and will complete with status Cancelled.