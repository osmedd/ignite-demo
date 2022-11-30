# CMakeList.txt : CMake project for DemoWorkerCPP, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.20)

project ("Worker")

set(CMAKE_MODULE_PATH "${CMAKE_CURRENT_SOURCE_DIR}/cmake")
message(STATUS "Found CMAKE_MODULE_PATH: ${CMAKE_MODULE_PATH}")

set(toolboxDir ${CMAKE_SOURCE_DIR}/../../toolbox)

set(igniteVersion "2.14.0")
set(openSslVersion "OpenSSL_1_1_1q")

if (WIN32)
    set(platform wntx64)
    set(compilerVersion vc16)
else()
    set(platform lnx64)
    set(compilerVersion gcc731-rhel8-glibc228)
endif()

set(localToolboxPlatDir ${toolboxDir}/${platform})

set(ENV{OPENSSL_ROOT_DIR} ${localToolboxPlatDir}/openssl/${openSslVersion}-${compilerVersion})
message(STATUS "Found OPENSSL_ROOT_DIR: $ENV{OPENSSL_ROOT_DIR}")

set(IGNITE_CPP_DIR ${localToolboxPlatDir}/apache-ignite-client/${igniteVersion})

find_package(Ignite REQUIRED)
find_package(Threads)
find_package(Java 1.8 REQUIRED)
find_package(JNI REQUIRED)
find_package(OpenSSL)

message(STATUS "IGNITE_INCLUDE_DIR: ${IGNITE_INCLUDE_DIR}")

include_directories(
    SYSTEM
    ${OPENSSL_INCLUDE_DIR}
    ${IGNITE_INCLUDE_DIR}
    ${JNI_INCLUDE_DIRS}
    )

# Add source to this project's executable.
add_executable (${PROJECT_NAME})

set_property(TARGET ${PROJECT_NAME} PROPERTY MSVC_RUNTIME_LIBRARY "MultiThreadedDLL")

target_sources(${PROJECT_NAME}
    PRIVATE "Worker.cxx"
    PRIVATE "Worker.hxx"
    PRIVATE "DataGrid.hxx"
    PRIVATE "DataGridIgnite.cxx"
    PRIVATE "DataGridIgnite.hxx"
    PRIVATE "WorkerStatus.cxx"
    PRIVATE "WorkerStatus.hxx"
    )

target_compile_definitions(${PROJECT_NAME}
    PUBLIC IPLIB=libvariabilityadaptor
    PUBLIC _HAS_AUTO_PTR_ETC
    )

target_link_libraries(${PROJECT_NAME}
    PRIVATE ${IGNITE_LIB}
    PRIVATE ${IGNITE_BINARY_LIB}
    PRIVATE ${IGNITE_COMMON_LIB}
    PRIVATE ${CMAKE_THREAD_LIBS_INIT}
    )

if (WIN32)
    target_link_libraries(${PROJECT_NAME}
        PRIVATE OpenSSL::applink
        PRIVATE OpenSSL::Crypto
        )
endif()

if (CMAKE_VERSION VERSION_GREATER 3.12)
    set_property(TARGET ${PROJECT_NAME} PROPERTY CXX_STANDARD 17)
endif()

# TODO: Add tests and install targets if needed.

if (WIN32)
    file(GLOB igniteSharedLibraries ${IGNITE_CPP_DIR}/bin/*${CMAKE_SHARED_LIBRARY_SUFFIX})
    file(GLOB opensslSharedLibraries "${OPENSSL_INCLUDE_DIR}/../bin/*${CMAKE_SHARED_LIBRARY_SUFFIX}")
    list(APPEND installLibraries ${opensslSharedLibraries})
else()
    file(GLOB igniteSharedLibraries ${IGNITE_CPP_DIR}/lib/*${CMAKE_SHARED_LIBRARY_SUFFIX})
endif()
list(APPEND installLibraries ${igniteSharedLibraries})
add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy ${installLibraries} $<TARGET_FILE_DIR:${PROJECT_NAME}>
    COMMAND ${CMAKE_COMMAND} -E copy ${CMAKE_SOURCE_DIR}/../../config-cpp-client.xml $<TARGET_FILE_DIR:${PROJECT_NAME}>
    COMMAND_EXPAND_LISTS)

install(TARGETS Worker DESTINATION ${CMAKE_SOURCE_DIR}/../../out/${platform}/Worker)
install(FILES ${installLibraries} DESTINATION ${CMAKE_SOURCE_DIR}/../../out/${platform}/Worker)
install(FILES ${CMAKE_SOURCE_DIR}/../../config-cpp-client.xml DESTINATION ${CMAKE_SOURCE_DIR}/../../out/${platform}/Worker)
