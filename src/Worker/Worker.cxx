// Worker.cxx : Defines the entry point for the application.
//

#include "Worker.hxx"
#include "DataGridIgnite.hxx"
#include <openssl/evp.h>
#include <signal.h>
#include <chrono>
#include <thread>

using namespace ignite;

int main(int argc, char* argv[])
{
	std::cerr << "Hello CMake." << std::endl;

	try
	{
		DataGridIgnite ignite;

		auto workerId = ignite.getWorkerId();
		std::cerr << "main: got workerId: " << workerId << std::endl;

		WorkerStatus workerStatus(workerId);

		while (true)
		{
			workerStatus.incrementRequestsServed();
			std::cerr << "main: requests served: " << workerStatus.m_requestsServed << std::endl;
			ignite.updateWorkerStatus(workerId, workerStatus);
			std::this_thread::sleep_for(std::chrono::milliseconds(60000));
		}
	}
	catch (const IgniteError& e)
	{
		std::cerr << "Caught exception: " << e.what() << std::endl;
	}
	std::cerr << "Node stopped." << std::endl;

	return 0;
}
