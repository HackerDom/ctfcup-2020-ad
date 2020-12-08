data "yandex_compute_image" "ubuntu-with-docker" {
  family = "ubuntu-with-docker"
  folder_id = "<данные удалены>"
}

locals {
  ubuntu-with-docker-id = data.yandex_compute_image.ubuntu-with-docker.id
}

locals {
  jury_ssh_key = file("${path.root}/../../teams/for_devs.ssh_key.pub")
}
