import grpc
from concurrent import futures
import calc_pb2
import calc_pb2_grpc
import logging

class GreetServicer(calc_pb2_grpc.GreeterServicer):
    def __init__(self) -> None:
        super().__init__

    def Double(self, request, context):
        return calc_pb2.HelloReply(value=2 * int(request.value))
    

def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    calc_pb2_grpc.add_GreeterServicer_to_server(GreetServicer(), server)
    server.add_insecure_port("[::]:50051")
    server.start()
    server.wait_for_termination()


if __name__ == '__main__':
    logging.basicConfig()
    serve()